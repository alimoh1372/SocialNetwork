using System.Collections.Generic;
using System.Threading.Tasks;
using _00_Framework.Domain;
using SocialNetwork.Application.Contracts.UserRelationContracts;

namespace SocialNetwork.Domain.UserRelationAgg
{
    /// <summary>
    /// To relate the entity with database and application
    /// </summary>
    public interface IUserRelationRepository:IBaseRepository<long,UserRelation>
    {
        Task<List<UserWithRequestStatusVieModel>> GetAllUserWithRequestStatus(long currentUserId);
        Task<UserRelation> GetRelationBy(long userIdRequestSentFromIt, long userIdRequestSentToIt);
    }
}