using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace mars_marking_svc.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetAsync(string request)
        {
            return await _httpClient.GetAsync(request);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string request, T updatedModel)
        {
            var httpContent = new StringContent(
                JsonConvert.SerializeObject(updatedModel),
                Encoding.UTF8,
                "application/json"
            );

            return await _httpClient.PutAsync(request, httpContent);
        }
    }
}