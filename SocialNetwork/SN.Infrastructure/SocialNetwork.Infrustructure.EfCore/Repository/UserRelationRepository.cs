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
            var query =await _context.Users.Include(x => x.UserARelations)
                .Include(x => x.UserBRelations)
                .Where(x => x.Id != currentUserId)
                .Select(x => new UserWithRequestStatusVieModel
                {
                    UserId = x.Id,
                    Name = x.Name,
                    LastName = x.LastName,
                    ProfilePicture = x.ProfilePicture
                    
                }).AsNoTracking()
                .ToListAsync();
            
            //Fill request status property
            foreach (UserWithRequestStatusVieModel userWithRequestStatusVieModel in query)
            {
                userWithRequestStatusVieModel.RequestStatusNumber =await CheckStatusOfRequest(currentUserId,
                    userWithRequestStatusVieModel.UserId);
            }
           
            return  query;
        }

        public async Task<UserRelation> GetRelationBy(long userIdRequestSentFromIt, long userIdRequestSentToIt)
        {
            return await _context.UserRelations.FirstOrDefaultAsync(x =>
                x.FkUserAId == userIdRequestSentFromIt && x.FkUserBId == userIdRequestSentToIt);
        }

        private async Task<RequestStatus> CheckStatusOfRequest(long userIdA, long userIdB)
        {
            //get relation between a to b or inversion of it
            var userRelations =await _context.UserRelations.
                Where(x => (x.FkUserAId == userIdA && x.FkUserBId == userIdB)
                                 || (x.FkUserAId == userIdB && x.FkUserBId == userIdA))
                                 .ToListAsync();

            //check if isn't any request return that status
            if (userRelations.Count == 0)
                return RequestStatus.WithoutRequest;
            //if relations is bigger than 1 there is a error in application logic
            if (userRelations.Count > 1)
                return RequestStatus.ErrorWithRelationNumbers;

            var userRelation = userRelations.First();


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