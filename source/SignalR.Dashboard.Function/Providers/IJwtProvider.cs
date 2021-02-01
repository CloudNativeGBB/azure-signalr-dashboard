using System.Collections.Generic;

namespace SignalR.Dashboard.Function.Providers
{
    public interface IJwtProvider
    {
        string GenerateToken(string authorizationKey);

        (bool, Dictionary<string, string>) ValidateToken(string token, string authorizationKey);
    }
}