using System;

namespace CsvUploader
{
    internal static partial class CsvUploaderCommands
    {
        private static string BuildConnectionString(string accountName, string accountKey)
            => $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}";

        private static string GetAzuriteConnectionString()
            => $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;" +
            $"AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
            $"BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1";

        public static string BuildConnectionString(ConnectionParameters parameters)
        {
            if (parameters.UseAzurite) return GetAzuriteConnectionString();
            else
            {
                if (string.IsNullOrEmpty(parameters.AccountName))
                {
                    throw new ArgumentException("Expecting account name", nameof(ConnectionParameters.AccountName));
                }

                if (string.IsNullOrEmpty(parameters.AccountKey))
                {
                    throw new ArgumentException("Expecting account key", nameof(ConnectionParameters.AccountKey));
                }

                return BuildConnectionString(parameters.AccountName, parameters.AccountKey);
            }
        }
    }
}
