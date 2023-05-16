using System.Threading.Tasks;
using SocialNetwork.Application.Contracts.UserContracts;

namespace _01_SocialNetworkQuery.Contract
{
    public interface IUserQuery
    {
        Task<UserViewModel> GetCurrentUserInfo();
    }
}