using Newtonsoft.Json;
using SignalR.Dashboard.Function.Models.Responses;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SignalR.Dashboard.Function.Helpers
{
    public static class ResponseBuilderHelper
    {
        public static HttpResponseMessage BuildResponse<D>(HttpStatusCode statusCode, D result = default)
        {
            if (result == null)
                return new HttpResponseMessage(statusCode);

            return new HttpResponseMessage(statusCode) { Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json") };
        }

        public static HttpResponseMessage BuildResponse(HttpStatusCode statusCode, string message = "")
        {
            var result = new MessageResponse();
            result.Message = message;

            if (string.IsNullOrEmpty(message))
                return new HttpResponseMessage(statusCode);

            return new HttpResponseMessage(statusCode) { Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json") };
        }
    }
}