using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _00_Framework.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Application.Contracts.UserContracts;

namespace ServiceHosts.Pages
{
    
    public class EditProfilePictureModel : PageModel
    {
        
        public EditProfilePicture EditProfilePicture { get; set; }
        private readonly IUserApplication _userApplication;

        public EditProfilePictureModel(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }

        public void OnGet(long id)
        {
            EditProfilePicture = _userApplication.GetEditProfileDetails(id).Result;
        }

        public async Task<IActionResult> OnPost(EditProfilePicture command)
        {
            OperationResult result = await _userApplication.ChangeProfilePicture(command);
            if (result.IsSuccedded)
                return RedirectToPage("/ChatPage");

           
            return Page();
        }
    }
}
