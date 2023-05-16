using System.Collections.Generic;
using System.Threading.Tasks;
using _00_Framework.Domain;
using SocialNetwork.Application.Contracts.UserRelationContracts;

namespace SocialNetwork.Domain.UserRelationAgg
{
    public interface IUserRelationRepository:IBaseRepository<long,UserRelation>
    {
        Task<List<UserWithRequestStatusVieModel>> GetAllUserWithRequestStatus(long currentUserId);
    }
}