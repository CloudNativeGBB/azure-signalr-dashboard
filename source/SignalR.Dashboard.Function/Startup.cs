using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SignalR.Dashboard.Function;
using SignalR.Dashboard.Function.Providers;

[assembly: FunctionsStartup(typeof(Startup))]

namespace SignalR.Dashboard.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
        }
    }
}