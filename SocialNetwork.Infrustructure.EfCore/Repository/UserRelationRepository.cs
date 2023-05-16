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
    public class UserRelationRepository:BaseRepository<long,UserRelation>,IUserRelationRepository
    {
        private readonly SocialNetworkContext _context;
        public UserRelationRepository(SocialNetworkContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UserWithRequestStatusVieModel>> GetAllUserWithRequestStatus(long currentUserId)
        {
            var query = _context.Users.Include(x => x.UserARelations)
                .Include(x => x.UserBRelations)
                .Where(x=>x.Id != currentUserId)
                .Select(x => new UserWithRequestStatusVieModel
                {
                    UserId = x.Id,
                    FullName = x.Name +" "+x.LastName,
                }).AsNoTracking()
                .ToListAsync();
            RequestStatusNumber = CheckStatusOfRequest(currentUserId, x.Id);
        }

        private  RequestStatus CheckStatusOfRequest(long userIdA, long userIdB)
        {
            var userRelation = _context.UserRelations.FirstOrDefaultAsync(x => x.FkUserAId == userIdA);
            if (userRelation.Result == null)
            {
                var revetRelation = _context.UserRelations.FirstOrDefaultAsync(x => x.FkUserAId == userIdA);
                if (revetRelation ==null)
                {
                    var
                }
            }
            
        }
    }
}