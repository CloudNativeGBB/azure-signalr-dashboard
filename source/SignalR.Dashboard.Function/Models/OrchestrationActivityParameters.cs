using SignalR.Dashboard.Function.Models.Requests;
using System.Collections.Generic;

namespace SignalR.Dashboard.Function.Models
{
    public class OrchestrationActivityParameters
    {
        public string AccessToken { get; set; }

        public CreateActivityRequest RequestData { get; set; }

        public Dictionary<string, string> Claims { get; set; }
    }
}