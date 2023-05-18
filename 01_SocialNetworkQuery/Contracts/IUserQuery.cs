using System.Threading.Tasks;
using SocialNetwork.Application.Contracts.UserContracts;

namespace _01_SocialNetworkQuery.Contracts
{
    public interface IUserQuery
    {
        Task<UserViewModel> GetCurrentUserInfo();
        
    }
}