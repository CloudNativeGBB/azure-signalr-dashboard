using Newtonsoft.Json;

namespace SignalR.Dashboard.Function.Models.Responses
{
    public class MessageResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}