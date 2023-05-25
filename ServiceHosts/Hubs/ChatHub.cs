using System;
using System.Security.Claims;
using System.Threading.Tasks;
using _00_Framework.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using SocialNetwork.Application.Contracts.MessageContracts;
using SocialNetwork.Application.Contracts.UserContracts;
using SocialNetwork.Application.Contracts.UserRelationContracts;

using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ServiceHosts.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserRelationApplication _userRelationApplication;
        private readonly IMessageApplication _messageApplication;
        public ChatHub(IUserRelationApplication userRelationApplication, IMessageApplication messageApplication)
        {
            _userRelationApplication = userRelationApplication;
            _messageApplication = messageApplication;
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

        public async Task SendMessage(long fromUserId, long toUserId, string message)
        {
            //Todo:Implementing send Message
            var command = new SendMessage
            {
                FkFromUserId = fromUserId,
                FkToUserId = toUserId,
                MessageContent = message
            };
            OperationResult result = _messageApplication.Send(command);
            if (result.IsSuccedded)
            {
               var messageViewModel= _messageApplication.GetLatestMessage(fromUserId, toUserId);
               var jsonMessage = JsonSerializer.Serialize(messageViewModel.Result);
                    await Clients.Users(fromUserId.ToString(), toUserId.ToString())
                    .SendAsync("addNewMessageToChatHistoryUlEl", jsonMessage);
            }
            else
            {
                await Clients.Caller.SendAsync("ShowError", result.Message);
            }
        }
    }
}