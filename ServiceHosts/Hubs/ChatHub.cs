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
    public class ChatHub:Hub
    {
        private readonly IUserRelationApplication _userRelationApplication;
        private readonly IAuthHelper _authHelper;
        public ChatHub(IUserRelationApplication userRelationApplication, IAuthHelper authHelper)
        {
            _userRelationApplication = userRelationApplication;
            _authHelper = authHelper;
        }

        public async  void SendUserRelationRequest(long currentUserId, long requestSendTo)
        {
            //CreateUserRelation relation = new CreateUserRelation
            //{
            //    FkUserAId = currentUserId,
            //    FkUserBId = requestSendTo
            //};
            //var result =_userRelationApplication.Create(relation);
            if (true)
            {
                try
                {
                   await Clients.All.SendAsync("updateRequestRowAddAcceptButton", currentUserId); 
                    //Clients.Caller.SendAsync("updateCurrentUserRow", requestSendTo);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                 
            }
                

        }
    }
}