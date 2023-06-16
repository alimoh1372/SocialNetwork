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
        public string ErrorMessage { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

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
        public IActionResult OnPostLogin(Login command)
        {
            var result = _userApplication.Login(command);
            if (result.IsSuccedded)
            {
                return RedirectToPage("/ChatPage");
            }

            ErrorMessage = result.Message;
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
            SuccessMessage = result.Message;
            if (result.IsSuccedded)
            {
                var loginCommand = new Login
                {
                    UserName = command.Email,
                    Password = command.Password
                };
                result = _userApplication.Login(loginCommand);

                if (result.IsSuccedded)
                    return RedirectToPage("/ChatPage");
                
                SuccessMessage = "Signed up successfully please now login...";
                return RedirectToPage("/Index");
            }

            ErrorMessage = "There is an error please try again...";
            return RedirectToPage("/Index");
        }

    }
}
