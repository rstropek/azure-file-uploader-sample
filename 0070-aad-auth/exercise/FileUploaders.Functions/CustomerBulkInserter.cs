using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace FileUploaders.Functions
{
    public interface ICustomerBulkInserter
    {
        Task StartAsync();
        void Add(Customer c);
        Task Insert();
    }

    public class CustomerBulkInserter : IAsyncDisposable, ICustomerBulkInserter
    {
        private static readonly string SqlConnection;

        private readonly DataColumn[] Columns;
        private SqlConnection? Connection;
        private DataTable? CustomerTable;
        private SqlBulkCopy? BulkCopy;
        private bool disposedValue;

        static CustomerBulkInserter()
        {
            SqlConnection = Environment.GetEnvironmentVariable("SqlConnection")!;
        }

        public CustomerBulkInserter()
        {
            Columns = new []
            {
                new DataColumn(nameof(Customer.Id), typeof(int)),
                new DataColumn(nameof(Customer.FirstName), typeof(string)),
                new DataColumn(nameof(Customer.LastName), typeof(string)),
                new DataColumn(nameof(Customer.Email), typeof(string)),
                new DataColumn(nameof(Customer.Gender), typeof(string)),
                new DataColumn(nameof(Customer.IpAddress), typeof(string)),
            };
        }

        public async Task StartAsync()
        {
            if (Connection == null)
            {
                Connection = new(SqlConnection);
                await Connection.OpenAsync();

                using var cmd = Connection.CreateCommand();
                cmd.CommandText = "TRUNCATE TABLE dbo.CustomersStaging";
                await cmd.ExecuteNonQueryAsync();
            }

            if (CustomerTable == null)
            {
                CustomerTable = new();
                CustomerTable.Columns.AddRange(Columns);
            }

            if (BulkCopy == null)
            {
                BulkCopy = new SqlBulkCopy(Connection) { DestinationTableName = "CustomersStaging" };
                BulkCopy.ColumnMappings.Add(nameof(Customer.Id), nameof(Customer.Id));
                BulkCopy.ColumnMappings.Add(nameof(Customer.FirstName), nameof(Customer.FirstName));
                BulkCopy.ColumnMappings.Add(nameof(Customer.LastName), nameof(Customer.LastName));
                BulkCopy.ColumnMappings.Add(nameof(Customer.Email), nameof(Customer.Email));
                BulkCopy.ColumnMappings.Add(nameof(Customer.Gender), nameof(Customer.Gender));
                BulkCopy.ColumnMappings.Add(nameof(Customer.IpAddress), nameof(Customer.IpAddress));
            }
        }

        public void Add(Customer c)
        {
            if (CustomerTable == null) throw new InvalidOperationException();

            var row = CustomerTable.NewRow();
            row[0] = c.Id;
            row[1] = c.FirstName;
            row[2] = c.LastName;
            row[3] = c.Email;
            row[4] = c.Gender;
            row[5] = c.IpAddress;
            CustomerTable.Rows.Add(row);
        }

        public async Task Insert()
        {
            if (BulkCopy == null || CustomerTable == null) throw new InvalidOperationException();
            await BulkCopy.WriteToServerAsync(CustomerTable);
            CustomerTable.Rows.Clear();
        }

        public async Task Completed()
        {
            await DisposeAsync(true);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (BulkCopy != null)
                    {
                        BulkCopy.Close();
                        BulkCopy = null;
                    }

                    if (Connection != null)
                    {
                        await Connection.CloseAsync();
                        await Connection.DisposeAsync();
                        Connection = null;
                    }
                }

                disposedValue = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
