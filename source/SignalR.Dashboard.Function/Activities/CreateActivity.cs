using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SignalR.Dashboard.Function.Helpers;
using SignalR.Dashboard.Function.Models;
using SignalR.Dashboard.Function.Models.Requests;
using SignalR.Dashboard.Function.Providers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SignalR.Dashboard.Function.Activities
{
    public class CreateActivity
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger _logger;

        public CreateActivity(IJwtProvider jwtProvider, ILoggerFactory loggerFactory)
        {
            _jwtProvider = jwtProvider;
            _logger = loggerFactory.CreateLogger<CreateActivity>();
        }

        [FunctionName(nameof(Create))]
        public async Task<HttpResponseMessage> Create(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "1.0/create")]
            HttpRequestMessage req, [DurableClient] IDurableOrchestrationClient starter)
        {
            var (isValidToken, claims) = _jwtProvider.ValidateToken(req.Headers.GetValues(HeaderNames.Authorization).FirstOrDefault(), Settings.AuthorizationKey);
            if (!isValidToken)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            var (response, requestData) = await ValidateRequestAsync(req);
            if (response.StatusCode != HttpStatusCode.OK)
                return response;

            var parameters = new OrchestrationActivityParameters()
            {
                AccessToken = req.Headers.GetValues(HeaderNames.Authorization).FirstOrDefault(),
                RequestData = (CreateActivityRequest)requestData,
                Claims = claims
            };

            string instanceID = await starter.StartNewAsync<string>(nameof(OrchestrationActivity.Orchestration), JsonConvert.SerializeObject(parameters));
            _logger.LogInformation($"Instance ID: '{instanceID}'");

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private async Task<(HttpResponseMessage, object)> ValidateRequestAsync(HttpRequestMessage req)
        {
            try
            {
                string content = await req.Content.ReadAsStringAsync();
                var requestData = JsonConvert.DeserializeObject<CreateActivityRequest>(content);

                if (requestData == null)
                    return (ResponseBuilderHelper.BuildResponse(HttpStatusCode.BadRequest, "There was an error processing the request"), default);

                if (string.IsNullOrEmpty(requestData.Name))
                    return (ResponseBuilderHelper.BuildResponse(HttpStatusCode.BadRequest, "Missing parameter: name"), default);

                return (ResponseBuilderHelper.BuildResponse(HttpStatusCode.OK), requestData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (ResponseBuilderHelper.BuildResponse(HttpStatusCode.BadRequest, "There was an error processing the request"), default);
            }
        }
    }
}