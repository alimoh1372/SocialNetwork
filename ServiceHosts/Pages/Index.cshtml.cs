using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Application;
using SocialNetwork.Application.Contracts.UserContracts;

namespace ServiceHosts.Pages
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string LoginMessage { get; set; }

        [TempData]
        public string RegisterMessage { get; set; }

        private readonly IUserApplication _userApplication;
        private readonly ILogger<IndexModel> _logger;
        
        public IndexModel(ILogger<IndexModel> logger, IUserApplication userApplication)
        {
            _logger = logger;
            _userApplication = userApplication;
        }
        public void OnGet()
        {

        }
        public  IActionResult OnPostLogin(Login command)
        {
            var result = _userApplication.Login(command);
            if ( result.IsSuccedded)
                return RedirectToPage("/ChatPage");

            LoginMessage = result.Message;
            return RedirectToPage("/Index");
        }
        public IActionResult OnGetLogout()
        {
            _userApplication.Logout();
            return RedirectToPage("/Index");
        }
        public IActionResult OnPostRegister(CreateUser command)
        {
            var result = _userApplication.Create(command);
            if (result.IsSuccedded)
                return RedirectToPage("/ChatPage");
            RegisterMessage = result.Message;
            return RedirectToPage("/Index");
        }
       
    }
}
