using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetwork.Application.Contracts.UserRelationContracts;

namespace _01_SocialNetworkQuery.Contracts
{
    public interface IUserRelationQuery
    {
        /// <summary>
        /// Get All user expect user with id<see cref="currentUserId"/> and show its relation status with current user
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        Task<List<UserWithRequestStatusVieModel>> GetAllUsersWithRelationStatusAsync(long currentUserId);
        /// <summary>
        /// Get All friend of user with id<paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns><see langworf="null"/> if user haven't any friend</returns>

        Task<List<UserWithRequestStatusVieModel>> GetFriendsOfUser(long userId);
    }
}