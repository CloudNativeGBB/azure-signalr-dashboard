using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Net.Http.Headers;
using SignalR.Dashboard.Function.Helpers;
using SignalR.Dashboard.Function.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SignalR.Dashboard.Function.Models
{
    public class NegotiateActivity
    {
        private const string HUB_NAME = "updates";
        private readonly IJwtProvider _jwtProvider;

        public NegotiateActivity(IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
        }

        [FunctionName(nameof(Negotiate))]
        public HttpResponseMessage Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "1.0/{userID}/negotiate")]
            HttpRequestMessage req, [SignalRConnectionInfo(HubName = HUB_NAME, UserId = "{userID}")] SignalRConnectionInfo connectionInfo)
        {
            req.Headers.TryGetValues(HeaderNames.Authorization, out IEnumerable<string> authorizationEnumerable);
            if (authorizationEnumerable == null)
                return ResponseBuilderHelper.BuildResponse(HttpStatusCode.Unauthorized);

            var authorizationList = authorizationEnumerable.ToList();
            if (authorizationList.Count == 0)
                return ResponseBuilderHelper.BuildResponse(HttpStatusCode.Unauthorized);

            var (isValidToken, claims) = _jwtProvider.ValidateToken(authorizationList[0], Settings.AuthorizationKey);
            if (!isValidToken)
                return ResponseBuilderHelper.BuildResponse(HttpStatusCode.Unauthorized);

            claims.TryGetValue("userID", out var userID);
            if (string.IsNullOrEmpty(userID))
                return ResponseBuilderHelper.BuildResponse(HttpStatusCode.BadRequest, "Missing parameter: userID");

            return ResponseBuilderHelper.BuildResponse(HttpStatusCode.OK, connectionInfo);
        }
    }
}