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

        public async void SendUserRelationRequest(long currentUserId, long requestSendToId,string relationMessage)
        {
            CreateUserRelation relation = new CreateUserRelation
            {
                FkUserAId = currentUserId,
                FkUserBId = requestSendToId,
                RelationRequestMessage = relationMessage
            };
            var result = _userRelationApplication.Create(relation);
            if (result.IsSuccedded)
            {
                try
                {
                    await Clients.User(requestSendToId.ToString()).SendAsync("updateRequestRowAddAcceptButton", currentUserId,relationMessage);
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
                var countMutualFriend =
                  await  _userRelationApplication.GetNumberOfMutualFriend(currentUserId, userIdRequestSentFromIt);
                await Clients.Users(currentUserId.ToString(), userIdRequestSentFromIt.ToString())
                    .SendAsync("handleAfterAcceptedRequest", userIdRequestSentFromIt, currentUserId,countMutualFriend);
                return;
            }

            if (!result.IsSuccedded)
                await Clients.Caller.SendAsync("ShowError", result.Message);
        }

        public async Task SendMessage(long fromUserId, long toUserId, string message)
        {

            var command = new SendMessage
            {
                FkFromUserId = fromUserId,
                FkToUserId = toUserId,
                MessageContent = message
            };
            OperationResult result = _messageApplication.Send(command);
            if (result.IsSuccedded)
            {
                var messageViewModel = _messageApplication.GetLatestMessage(fromUserId, toUserId);
                var jsonMessage = JsonSerializer.Serialize(messageViewModel.Result);
                await Clients.Users(fromUserId.ToString(), toUserId.ToString())
                .SendAsync("addNewMessageToChatHistoryUlEl", jsonMessage);
            }
            else
            {
                await Clients.Caller.SendAsync("ShowError", result.Message);
            }
        }
        /// <summary>
        /// Edit the message with id=<paramref name="id"/> and new message=<paramref name="messageContext"/>
        /// </summary>
        /// <param name="id">message id</param>
        /// <param name="messageContext">message content change to it</param>
        /// <returns></returns>
        public async Task EditMessage(long id, string messageContext)
        {
            //Get the edit model of message
            var message = await _messageApplication.GetEditMessageBy(id);

            //check if it isn't exist
            if (message == null)
            {
                await Clients.Caller.SendAsync("ShowError", ApplicationMessage.NotFound);
                return;
            }
            //renew the message
            message.MessageContent = messageContext;
            

            //Go to operate editing
            var result = _messageApplication.Edit(message);

            if (result.IsSuccedded)
            {
                var editedMessage =await _messageApplication.GetMessageViewModelBy(id);
                //Send the response to the clients ui
                var jsonMessage = JsonSerializer.Serialize(editedMessage);
                await Clients.Users(message.FkFromUserId.ToString(), message.FkToUserId.ToString())
                    .SendAsync("EditMessage", message);

            }
            else
            {
                //if happening any error show the error on client side
                await Clients.Caller.SendAsync("ShowError", result.Message);
            }
        }
    }
}