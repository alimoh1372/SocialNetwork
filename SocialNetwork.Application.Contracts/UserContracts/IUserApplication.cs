using System.Collections.Generic;
using System.Threading.Tasks;
using _00_Framework.Application;

namespace SocialNetwork.Application.Contracts.UserContracts
{
    public interface IUserApplication
    {

        OperationResult Create(CreateUser command);
        OperationResult Edit(EditUser command);

        OperationResult ChangePassword(ChangePassword command);
        EditUser GetDetails(long id);

       Task<List<UserViewModel>> SearchAsync(SearchModel searchModel);


       OperationResult Login(Login command);
       void Logout();
      
    }

   

    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}