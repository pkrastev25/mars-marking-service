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

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await _httpClient.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string requestUri, T updatedModel)
        {
            var httpContent = new StringContent(
                JsonConvert.SerializeObject(updatedModel),
                Encoding.UTF8,
                "application/json"
            );

            return await _httpClient.PutAsync(requestUri, httpContent);
        }

        public async Task<HttpResponseMessage> PatchAsync<T>(string requestUri, T updatedModel)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(updatedModel),
                    Encoding.UTF8,
                    "application/json"
                )
            };

            return await _httpClient.SendAsync(request);
        }
    }
}