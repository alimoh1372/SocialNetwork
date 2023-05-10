using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _00_Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.Contracts.UserContracts;
using SocialNetwork.Domain.UserAgg;

namespace SocialNetwork.Infrastructure.EfCore.Repository
{
    public class UserRepository:BaseRepository<long,User>,IUserRepository
    {
        private readonly SocialNetworkContext _context;
        public UserRepository( SocialNetworkContext context) : base(context)
        {
            _context = context;
        }

        public EditUser GetDetails(long id)
        {
            return _context.Users.Select(x => new EditUser
            {
                Id = x.Id,
                Name = x.Name,
                LastName = x.LastName,
                AboutMe = x.AboutMe,
                ProfilePicture = x.ProfilePicture
            }).FirstOrDefault(x=>x.Id==id);
        }

        public Task<List<UserViewModel>> SearchAsync(SearchModel searchModel)
        {
            var query = _context.Users.Select(x => new UserViewModel
            {
                Id = x.Id,
                Email = x.Email,
                ProfilePicture = x.ProfilePicture
            });
            if (!string.IsNullOrWhiteSpace(searchModel.Email))
                query = query.Where(x => x.Email == searchModel.Email);

            return  query.ToListAsync();

        }
    }
}