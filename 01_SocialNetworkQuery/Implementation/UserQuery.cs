using System.Threading.Tasks;
using _00_Framework.Application;
using _01_SocialNetworkQuery.Contract;
using SocialNetwork.Application.Contracts.UserContracts;

namespace _01_SocialNetworkQuery.Implementation
{
    public class UserQuery :IUserQuery
    {
        private readonly IAuthHelper _authHelper;
        private readonly IUserApplication _userApplication;
        public UserQuery(IAuthHelper authHelper, IUserApplication userApplication)
        {
            _authHelper = authHelper;
            _userApplication = userApplication;
        }

        public async Task<UserViewModel> GetCurrentUserInfo()
        {
            if (!_authHelper.IsAuthenticated())
            {
                _authHelper.SignOut();
            }

            var user=await _userApplication.GetUserInfoAsyncBy(_authHelper.CurrentAccountInfo().Id);
            return   user;
        }
    }
}