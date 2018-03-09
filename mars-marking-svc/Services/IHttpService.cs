using System.Net.Http;
using System.Threading.Tasks;

namespace mars_marking_svc.Services
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> GetAsync(string request);

        Task<HttpResponseMessage> PutAsync<T>(string request, T updatedModel);
    }
}