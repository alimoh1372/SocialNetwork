using System.Collections.Generic;
using System.Threading.Tasks;
using _01_SocialNetworkQuery.Contracts;
using SocialNetwork.Application.Contracts.UserRelationContracts;

namespace _01_SocialNetworkQuery.Implementations
{
    public class UserRelationQuery : IUserRelationQuery
    {
        private readonly IUserRelationApplication _userRelationApplication;

        public UserRelationQuery(IUserRelationApplication userRelationApplication)
        {
            _userRelationApplication = userRelationApplication;
        }

        public async Task<List<UserWithRequestStatusVieModel>> GetAllUsersWithRelationStatusAsync(long currentUserId)
        {
            return await _userRelationApplication.GetAllUserWithRequestStatus(currentUserId);
        }
    }
}