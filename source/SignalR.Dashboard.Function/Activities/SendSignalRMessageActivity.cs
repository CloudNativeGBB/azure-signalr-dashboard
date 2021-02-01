using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using SignalR.Dashboard.Function.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalR.Dashboard.Function.Activities
{
    public class SendSignalRMessageActivity
    {
        private const string HUB_NAME = "updates";
        private readonly ILogger _logger;

        public SendSignalRMessageActivity(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendSignalRMessageActivity>();
        }

        [FunctionName(nameof(SendSignalRMessage))]
        public async Task<bool> SendSignalRMessage(
            [ActivityTrigger] IDurableActivityContext context, [SignalR(HubName = HUB_NAME)] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var (claims, content, messageType) = context.GetInput<(Dictionary<string, string>, string, string)>();
            claims.TryGetValue("userID", out var userID);
            if (string.IsNullOrEmpty(userID))
            {
                _logger.LogError($"There was an error getting the user identifier");
                return false;
            }

            var signalRMessage = new SendSignalRMessageActivityResponse()
            {
                MessageType = messageType,
                Content = content
            };

            await signalRMessages.AddAsync(new SignalRMessage
            {
                UserId = userID,
                Target = "sendUpdate", // client method name
                Arguments = new[] { signalRMessage }
            });

            return true;
        }
    }
}