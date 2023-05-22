using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace ServiceHosts.Tools.Implementation
{
    /// <summary>
    /// To change the identity of user from <c>name</c> into <c>UserId</c>
    /// </summary>
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