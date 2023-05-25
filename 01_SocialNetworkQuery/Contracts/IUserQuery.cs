using System.Threading.Tasks;
using SocialNetwork.Application.Contracts.UserContracts;


namespace _01_SocialNetworkQuery.Contracts
{
    /// <summary>
    /// ready the queries of application we need 
    /// </summary>
    public interface IUserQuery
    {
        /// <summary>
        /// Get the current user information
        /// </summary>
        /// <returns></returns>
        Task<UserViewModel> GetCurrentUserInfo();
        
    }
}