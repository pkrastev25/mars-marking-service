using Newtonsoft.Json;

namespace mars_marking_svc.Middlewares.Models
{
    public class ErrorMessageModel
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        public ErrorMessageModel(
            string error
        )
        {
            Error = error;
        }
    }
}