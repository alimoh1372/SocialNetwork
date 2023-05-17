using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _00_Framework.Application;
using _00_Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.Contracts.UserRelationContracts;
using SocialNetwork.Domain.UserRelationAgg;

namespace SocialNetwork.Infrastructure.EfCore.Repository
{
    public class UserRelationRepository : BaseRepository<long, UserRelation>, IUserRelationRepository
    {
        private readonly SocialNetworkContext _context;
        public UserRelationRepository(SocialNetworkContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// Get the All user with request status  to another users
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<List<UserWithRequestStatusVieModel>> GetAllUserWithRequestStatus(long currentUserId)
        {
            //Get all user expect current user
            var query = _context.Users.Include(x => x.UserARelations)
                .Include(x => x.UserBRelations)
                .Where(x => x.Id != currentUserId)
                .Select(x => new UserWithRequestStatusVieModel
                {
                    UserId = x.Id,
                    Name = x.Name,
                    LastName = x.LastName,
                    
                }).AsNoTracking()
                .ToListAsync();
            
            //Fill request status property
            Parallel.ForEach( query.Result,userRelation =>
            {
                userRelation.RequestStatusNumber = CheckStatusOfRequest(currentUserId, userRelation.UserId);
            });
            return await query;
        }

        private RequestStatus CheckStatusOfRequest(long userIdA, long userIdB)
        {
            //get relation between a to b or inversion of it
            var userRelations = _context.UserRelations.
                Where(x => (x.FkUserAId == userIdA && x.FkUserBId == userIdB)
                                 || (x.FkUserAId == userIdB && x.FkUserBId == userIdA))
                                 .ToListAsync();

            //check if isn't any request return that status
            if (userRelations.Result.Count == 0)
                return RequestStatus.WithoutRequest;
            //if relations is bigger than 1 there is a error in application logic
            if (userRelations.Result.Count > 1)
                return RequestStatus.ErrorWithRelationNumbers;

            var userRelation = userRelations.Result.First();


            if (userRelation.FkUserAId == userIdA && userRelation.FkUserBId == userIdB && userRelation.Approve == false)
                return RequestStatus.RequestPending;

            if (userRelation.FkUserAId == userIdA && userRelation.FkUserBId == userIdB && userRelation.Approve == true)
                return RequestStatus.RequestAccepted;

            if (userRelation.FkUserAId == userIdB && userRelation.FkUserBId == userIdA && userRelation.Approve == false)
                return RequestStatus.RevertRequestPending;

            if (userRelation.FkUserAId == userIdB && userRelation.FkUserBId == userIdA && userRelation.Approve == true)
                return RequestStatus.RevertRequestAccepted;

            return RequestStatus.UnknownError;
        }
    }
}