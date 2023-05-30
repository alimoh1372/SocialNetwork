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
            var query = await _context.Users.Include(x => x.UserARelations)
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
                userWithRequestStatusVieModel.RequestStatusNumber = await CheckStatusOfRequest(currentUserId,
                    userWithRequestStatusVieModel.UserId);

                //Get the request Message
                if (userWithRequestStatusVieModel.RequestStatusNumber == RequestStatus.RequestPending ||
                    userWithRequestStatusVieModel.RequestStatusNumber == RequestStatus.RevertRequestPending)
                {
                    userWithRequestStatusVieModel.RelationRequestMessage =
                        await GetRequestMessage(currentUserId, userWithRequestStatusVieModel.UserId);
                }
                //set number of mutual friend 
                if (userWithRequestStatusVieModel.RequestStatusNumber == RequestStatus.RequestAccepted ||
                    userWithRequestStatusVieModel.RequestStatusNumber == RequestStatus.RevertRequestAccepted)
                {
                    userWithRequestStatusVieModel.MutualFriendNumber =
                        await GetMutualFriendNumber(currentUserId, userWithRequestStatusVieModel.UserId);
                }
            }

            return query;
        }

        //Find number of mutual friend
        public async Task<int> GetMutualFriendNumber(long currentUserId, long friendUserId)
        {
            #region Way1Ofsolve this

            ////Get a list of friendShip of friend with user id friendUserId
            //var listOfUserIdFriendShip = await _context.UserRelations.Where(x => x.Approve && (x.FkUserAId == friendUserId || x.FkUserBId == friendUserId))
            //    .ToListAsync();

            ////Get a list of friendShip of current User
            //var listOfCurrentUserIdFriendShip = await _context.UserRelations.Where(x =>
            //    x.Approve && (currentUserId == x.FkUserAId || currentUserId == x.FkUserBId)).ToListAsync();


            //int mutualFriends = (int)0;
            //Parallel.ForEach(listOfUserIdFriendShip, (userRelation) =>
            //{
            //    //Find the friend of a friend id to check it is also friend with current user?
            //    var otherFriendId = userRelation.FkUserAId == friendUserId ? userRelation.FkUserBId : userRelation.FkUserAId;


            //    //Check There is any relation between current user and other friend of current user's friend
            //    bool isExistAFriendShipWithCurrentUser =
            //        listOfCurrentUserIdFriendShip.Any(x =>
            //            x.FkUserAId == otherFriendId || x.FkUserBId == otherFriendId);

            //    //Add number of friend
            //    if (isExistAFriendShipWithCurrentUser)
            //    {
            //        mutualFriends++;
            //    }
            //});
            //return mutualFriends;

            #endregion

            #region OtherWayToSolveThisProblem
            //Get List of id of friend of current user id
            var currentUserFriendsId = _context.UserRelations.Where(x =>
                x.Approve && (currentUserId == x.FkUserAId || currentUserId == x.FkUserBId))
                .Select(x => new
                {
                    Id = x.FkUserAId==currentUserId?x.FkUserBId:x.FkUserAId
                });
            var userIdFriendsIs = _context.UserRelations.Where(x =>
                    x.Approve && (friendUserId == x.FkUserAId || friendUserId == x.FkUserBId))
                .Select(x => new
                {
                    Id = x.FkUserAId == friendUserId ? x.FkUserBId : x.FkUserAId
                });
            //Find Count of common id in two list
            var countMutualFriend =await currentUserFriendsId.Intersect(userIdFriendsIs).CountAsync();
            return countMutualFriend;

            #endregion

        }

        private async Task<string> GetRequestMessage(long userIdA, long userIdB)
        {
            //get relation between a to b or inversion of it
            var userRelation = await _context.UserRelations.
                Where(x => (x.FkUserAId == userIdA && x.FkUserBId == userIdB)
                           || (x.FkUserAId == userIdB && x.FkUserBId == userIdA))
                .FirstOrDefaultAsync();
            if (userRelation == null)
                return "Relation not found";
            return userRelation.RelationRequestMessage;
        }

        public async Task<UserRelation> GetRelationBy(long userIdRequestSentFromIt, long userIdRequestSentToIt)
        {
            return await _context.UserRelations.FirstOrDefaultAsync(x =>
                x.FkUserAId == userIdRequestSentFromIt && x.FkUserBId == userIdRequestSentToIt);
        }

        public async Task<List<UserWithRequestStatusVieModel>> GetFriendsOfUser(long userId)
        {
            return await _context.UserRelations
                .Include(x => x.UserA)
                .Include(x => x.UserB)
                //Filter All relation that accepted and user with id=friendUserId participate in it
                .Where(x => (x.UserA.Id == userId || x.UserB.Id == userId) && x.Approve)
                .Select(x => new UserWithRequestStatusVieModel
                {
                    //Set the userWith relation status to show the other users that friend with him and show other users in relation info
                    UserId = x.FkUserAId == userId ? x.FkUserBId : x.FkUserAId,
                    Name = x.FkUserAId == userId ? x.UserB.Name : x.UserA.Name,
                    LastName = x.FkUserAId == userId ? x.UserB.LastName : x.UserA.LastName,
                    //show that this user(otherUser) is requested the relation or not
                    RequestStatusNumber = x.FkUserAId == userId ? RequestStatus.RequestAccepted : RequestStatus.RevertRequestAccepted,
                    TimeOffset = x.CreationDate,
                    ProfilePicture = x.FkUserAId == userId ? x.UserB.ProfilePicture : x.UserA.ProfilePicture,
                })
                .ToListAsync();
        }

        private async Task<RequestStatus> CheckStatusOfRequest(long userIdA, long userIdB)
        {
            //get relation between a to b or inversion of it
            var userRelations = await _context.UserRelations.
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