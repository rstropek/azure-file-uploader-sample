using Azure.Storage;

namespace CsvUploader
{
    internal static partial class CsvUploaderCommands
    {
        private static string BuildConnectionString(string accountName, string accountKey)
            => $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}";

        private const string AzuriteAccountName = "devstoreaccount1";
        private const string AzuriteAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

        private static string GetAzuriteConnectionString()
            => $"DefaultEndpointsProtocol=http;AccountName={AzuriteAccountName};" +
            $"AccountKey={AzuriteAccountKey};" +
            $"BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1";

        public static string BuildConnectionString(ConnectionParameters parameters)
        {
            if (parameters.UseAzurite) return GetAzuriteConnectionString();
            else
            {
                parameters.EnsureNameAndKeyAreSet();
                return BuildConnectionString(parameters.AccountName, parameters.AccountKey);
            }
        }
    }
}
