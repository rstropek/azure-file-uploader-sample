using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace FileUploader.Shared
{
    public class FileUploaderHttpClient
    {
        private readonly HttpClient httpClient;

        public FileUploaderHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<GetUploadSasResultDto> GetUploadSas()
        {
            return await httpClient.GetFromJsonAsync<GetUploadSasResultDto>("GetUploadSas");
        }
    }
}
