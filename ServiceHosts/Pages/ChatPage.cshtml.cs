using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _01_SocialNetworkQuery.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Application.Contracts.UserContracts;
using SocialNetwork.Application.Contracts.UserRelationContracts;

namespace ServiceHosts.Pages
{
    
    public class ChatPageModel : PageModel
    {
        public UserViewModel UserInfo { get; set; }
        public List<UserWithRequestStatusVieModel> UserWithRequests { get; set; } 
        private readonly IUserQuery _userQuery;
        private readonly IUserRelationQuery _userRelationQuery;

        public ChatPageModel(IUserQuery userQuery, IUserRelationQuery userRelationQuery)
        {
            _userQuery = userQuery;
            _userRelationQuery = userRelationQuery;
        }

        public async void OnGet()
        {
            UserInfo =_userQuery.GetCurrentUserInfo().Result;
            UserWithRequests = await _userRelationQuery.GetAllUsersWithRelationStatusAsync(UserInfo.Id);
        }
    }
}
