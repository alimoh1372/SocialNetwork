using System;
using System.Security.Claims;
using System.Threading.Tasks;
using _00_Framework.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Application.Contracts.UserContracts;
using SocialNetwork.Application.Contracts.UserRelationContracts;

namespace ServiceHosts.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserRelationApplication _userRelationApplication;
        private readonly IAuthHelper _authHelper;
        public ChatHub(IUserRelationApplication userRelationApplication, IAuthHelper authHelper)
        {
            _userRelationApplication = userRelationApplication;
            _authHelper = authHelper;
        }

        public async void SendUserRelationRequest(long currentUserId, long requestSendToId)
        {
            CreateUserRelation relation = new CreateUserRelation
            {
                FkUserAId = currentUserId,
                FkUserBId = requestSendToId
            };
            var result = _userRelationApplication.Create(relation);
            if (result.IsSuccedded)
            {
                try
                {
                    await Clients.User(requestSendToId.ToString()).SendAsync("updateRequestRowAddAcceptButton", currentUserId);
                    await Clients.Caller.SendAsync("updateRequestRowAddPending", requestSendToId);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await Clients.Caller.SendAsync("ShowError", e.Message);
                    return;
                }

            }

            if (result.IsSuccedded)
                await Clients.Caller.SendAsync("ShowError", result.Message);

        }

        public async Task AcceptRequest(long currentUserId, long userIdRequestSentFromIt)
        {

            var result = await _userRelationApplication.Accept(userIdRequestSentFromIt, currentUserId);

            if (result.IsSuccedded)
            {
                await Clients.Users(currentUserId.ToString(), userIdRequestSentFromIt.ToString())
                    .SendAsync("handleAfterAcceptedRequest", userIdRequestSentFromIt, currentUserId);
                return;
            }

            if (!result.IsSuccedded)
                await Clients.Caller.SendAsync("ShowError", result.Message);
        }
    }
}