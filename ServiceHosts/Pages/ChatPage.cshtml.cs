using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _01_SocialNetworkQuery.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Application.Contracts.MessageContracts;
using SocialNetwork.Application.Contracts.UserContracts;
using SocialNetwork.Application.Contracts.UserRelationContracts;

namespace ServiceHosts.Pages
{
    
    [Authorize]
    public class ChatPageModel : PageModel
    {
        public UserViewModel UserInfo { get; set; }
        public List<UserWithRequestStatusVieModel> UserWithRequests { get; set; } 
        private readonly IUserQuery _userQuery;
        private readonly IUserRelationQuery _userRelationQuery;
        private readonly IMessageQuery _messageQuery;

        public ChatPageModel(IUserQuery userQuery, IUserRelationQuery userRelationQuery, IMessageQuery messageQuery)
        {
            _userQuery = userQuery;
            _userRelationQuery = userRelationQuery;
            _messageQuery = messageQuery;
        }

        public async void OnGet()
        {
            UserInfo =_userQuery.GetCurrentUserInfo().Result;
            UserWithRequests = await _userRelationQuery.GetAllUsersWithRelationStatusAsync(UserInfo.Id);
        }

        public async Task<JsonResult> OnGetLoadChatHistory(long currentUserId, long activeUserToChat)
        {
            var result= await _messageQuery.LoadChatHistory(currentUserId, activeUserToChat);
            return new JsonResult(result);
        }
    }
}
