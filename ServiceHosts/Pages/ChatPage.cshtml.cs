using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _01_SocialNetworkQuery.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Application.Contracts.UserContracts;

namespace ServiceHosts.Pages
{
    
    public class ChatPageModel : PageModel
    {
        public UserViewModel UserInfo { get; set; }
        public List<UserRequstViewModel> UserRequest { get; set; } 
        private readonly IUserQuery _userQuery;

        public ChatPageModel(IUserQuery userQuery)
        {
            _userQuery = userQuery;
        }

        public void OnGet()
        {
            UserInfo =_userQuery.GetCurrentUserInfo().Result;
        }
    }
}
