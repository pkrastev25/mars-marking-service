using System.Net.Http;
using System.Threading.Tasks;

namespace mars_marking_svc.Services.Models
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);

        Task<HttpResponseMessage> PutAsync<T>(string requestUri, T updatedModel);

        Task<HttpResponseMessage> PatchAsync<T>(string requestUri, T updatedModel);
    }
}