using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace mars_marking_svc.Utils
{
    public static class HttpResponseMessageExtensionsUtil
    {
        public static async Task<TModel> Deserialize<TModel>(
            this HttpResponseMessage httpResponseMessage
        )
        {
            var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TModel>(jsonResponse);
        }

        public static void ThrowExceptionIfNotSuccessfulResponse(
            this HttpResponseMessage httpResponseMessage,
            Exception exception
        )
        {
            if (httpResponseMessage == null)
            {
                throw new ArgumentNullException(nameof(httpResponseMessage));
            }

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw exception;
            }
        }
    }
}