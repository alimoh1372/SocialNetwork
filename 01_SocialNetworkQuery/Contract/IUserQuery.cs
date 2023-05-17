using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetwork.Application.Contracts.UserContracts;
using SocialNetwork.Application.Contracts.UserRelationContracts;

namespace _01_SocialNetworkQuery.Contract
{
    public interface IUserQuery
    {
        Task<UserViewModel> GetCurrentUserInfo();
        
    }
}