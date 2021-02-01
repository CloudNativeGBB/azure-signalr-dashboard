using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SignalR.Dashboard.Function.Helpers;
using SignalR.Dashboard.Function.Models.Responses;
using SignalR.Dashboard.Function.Providers;
using System.Net.Http;

namespace SignalR.Dashboard.Function.Activities
{
    public class AuthenticateActivity
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger _logger;

        public AuthenticateActivity(IJwtProvider jwtProvider, ILoggerFactory loggerFactory)
        {
            _jwtProvider = jwtProvider;
            _logger = loggerFactory.CreateLogger<AuthenticateActivity>();
        }

        [FunctionName(nameof(Authenticate))]
        public HttpResponseMessage Authenticate(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "1.0/authenticate")]
            HttpRequestMessage req)
        {
            return ResponseBuilderHelper.BuildResponse(System.Net.HttpStatusCode.OK, new AuthenticateActivityResponse() { Token = _jwtProvider.GenerateToken(Settings.AuthorizationKey) });
        }
    }
}