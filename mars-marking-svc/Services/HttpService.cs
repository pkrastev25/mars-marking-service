using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using mars_marking_svc.Services.Models;
using Newtonsoft.Json;

namespace mars_marking_svc.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(
            HttpClient httpClient
        )
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string requestUri, T newModel)
        {
            return await ExecuteRequest(
                _httpClient.PostAsync(requestUri, CreateStringContent(newModel))
            );
        }

        public async Task<HttpResponseMessage> GetAsync(
            string requestUri
        )
        {
            return await ExecuteRequest(
                _httpClient.GetAsync(requestUri)
            );
        }

        public async Task<HttpResponseMessage> PutAsync<T>(
            string requestUri,
            T updatedModel
        )
        {
            return await ExecuteRequest(
                _httpClient.PutAsync(requestUri, CreateStringContent(updatedModel))
            );
        }

        public async Task<HttpResponseMessage> PatchAsync<T>(
            string requestUri,
            T updatedModel
        )
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = CreateStringContent(updatedModel)
            };

            return await ExecuteRequest(
                _httpClient.SendAsync(request)
            );
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return await ExecuteRequest(
                _httpClient.DeleteAsync(requestUri)
            );
        }

        private StringContent CreateStringContent<T>(
            T updatedModel
        )
        {
            return new StringContent(
                JsonConvert.SerializeObject(updatedModel),
                Encoding.UTF8,
                "application/json"
            );
        }

        private async Task<HttpResponseMessage> ExecuteRequest(
            Task<HttpResponseMessage> request
        )
        {
            try
            {
                return await request;
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(e.InnerException?.Message))
                {
                    throw new Exception($"{e.Message} {e.InnerException.Message}");
                }

                throw;
            }
        }
    }
}