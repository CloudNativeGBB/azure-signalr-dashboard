using Newtonsoft.Json;

namespace SignalR.Dashboard.Function.Models.Requests
{
    public class CreateActivityRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}