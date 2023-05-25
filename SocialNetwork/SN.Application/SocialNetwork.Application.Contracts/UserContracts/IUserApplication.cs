using System.Collections.Generic;
using System.Threading.Tasks;
using _00_Framework.Application;

namespace SocialNetwork.Application.Contracts.UserContracts
{
    /// <summary>
    /// A Abstraction to implement user logic using other layers
    /// </summary>
    public interface IUserApplication
    {
      
        OperationResult Create(CreateUser command);
        OperationResult Edit(EditUser command);

        OperationResult ChangePassword(ChangePassword command);
        EditUser GetDetails(long id);
        /// <summary>
        /// Filter users with the <paramref name="searchModel"/>
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
       Task<List<UserViewModel>> SearchAsync(SearchModel searchModel);


       OperationResult Login(Login command);
       void Logout();
        /// <summary>
        /// Get the user info by id of user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       Task<UserViewModel> GetUserInfoAsyncBy(long id);
    }

   

    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}