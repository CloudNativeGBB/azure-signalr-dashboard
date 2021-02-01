using Newtonsoft.Json;

namespace SignalR.Dashboard.Function.Models
{
    public class SendSignalRMessageActivityResponse
    {
        [JsonProperty("messageType")]
        public string MessageType { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}