using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace ServiceHosts.Tools.Implementation
{
    public class UserIdProvider:IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            if (connection.User != null)
                return connection.User.FindFirst("UserId")?.Value;
            return "0";
        }
    }
}