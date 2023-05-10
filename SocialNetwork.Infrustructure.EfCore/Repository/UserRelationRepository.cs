using _00_Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
    }
}