using Newtonsoft.Json;

namespace SignalR.Dashboard.Function.Models.Responses
{
    public class AuthenticateActivityResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}