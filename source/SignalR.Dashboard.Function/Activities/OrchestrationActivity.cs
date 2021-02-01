using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SignalR.Dashboard.Function.Models;
using System.Threading.Tasks;

namespace SignalR.Dashboard.Function.Activities
{
    public class OrchestrationActivity
    {
        private readonly ILogger _logger;

        public OrchestrationActivity(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OrchestrationActivity>();
        }

        [FunctionName(nameof(Orchestration))]
        public async Task<bool> Orchestration(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            if (!context.IsReplaying) _logger.LogInformation("Starting orchestration");

            var parameters = JsonConvert.DeserializeObject<OrchestrationActivityParameters>(context.GetInput<string>());

            // NOTE: this is a sequential execution sample of tasks, you can replace this execution with a fan-out/fan-in pattern if needed

            await context.CallActivityAsync<bool>(nameof(SendSignalRMessageActivity.SendSignalRMessage), (parameters.Claims, "Creating records in database", "message"));
            bool createRecordResponse = await context.CallActivityAsync<bool>(nameof(CreateRecord), null);
            if (createRecordResponse)
                await context.CallActivityAsync<bool>(nameof(SendSignalRMessageActivity.SendSignalRMessage), (parameters.Claims, "25", "progress"));

            await context.CallActivityAsync<bool>(nameof(SendSignalRMessageActivity.SendSignalRMessage), (parameters.Claims, "Creating groups in active directory", "message"));
            bool createGroupsResponse = await context.CallActivityAsync<bool>(nameof(CreateGroups), null);
            if (createGroupsResponse)
                await context.CallActivityAsync<bool>(nameof(SendSignalRMessageActivity.SendSignalRMessage), (parameters.Claims, "50", "progress"));

            await context.CallActivityAsync<bool>(nameof(SendSignalRMessageActivity.SendSignalRMessage), (parameters.Claims, "Sending welcome email", "message"));
            bool sendWelcomeMailResponse = await context.CallActivityAsync<bool>(nameof(SendWelcomeMail), null);
            if (sendWelcomeMailResponse)
                await context.CallActivityAsync<bool>(nameof(SendSignalRMessageActivity.SendSignalRMessage), (parameters.Claims, "75", "progress"));

            await context.CallActivityAsync<bool>(nameof(SendSignalRMessageActivity.SendSignalRMessage), (parameters.Claims, "Notificating owners", "message"));
            bool notifyCreatorsResponse = await context.CallActivityAsync<bool>(nameof(NotifyCreators), null);
            if (notifyCreatorsResponse)
                await context.CallActivityAsync<bool>(nameof(SendSignalRMessageActivity.SendSignalRMessage), (parameters.Claims, "100", "progress"));

            await context.CallActivityAsync<bool>(nameof(SendSignalRMessageActivity.SendSignalRMessage), (parameters.Claims, "Process completed", "completed"));

            return true;
        }

        [FunctionName(nameof(CreateRecord))]
        public bool CreateRecord([ActivityTrigger] IDurableActivityContext context)
        {
            Task.Delay(1000).Wait();
            return true;
        }

        [FunctionName(nameof(CreateGroups))]
        public bool CreateGroups([ActivityTrigger] IDurableActivityContext context)
        {
            Task.Delay(2000).Wait();
            return true;
        }

        [FunctionName(nameof(SendWelcomeMail))]
        public bool SendWelcomeMail([ActivityTrigger] IDurableActivityContext context)
        {
            Task.Delay(4000).Wait();
            return true;
        }

        [FunctionName(nameof(NotifyCreators))]
        public bool NotifyCreators([ActivityTrigger] IDurableActivityContext context)
        {
            Task.Delay(2000).Wait();
            return true;
        }
    }
}