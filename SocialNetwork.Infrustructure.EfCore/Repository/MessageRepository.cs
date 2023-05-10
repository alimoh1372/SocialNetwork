using _00_Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Domain.MessageAgg;

namespace SocialNetwork.Infrastructure.EfCore.Repository
{
    public class MessageRepository:BaseRepository<long,Message>,IMessageRepository
    {
        private readonly SocialNetworkContext _context;
        public MessageRepository( SocialNetworkContext context) : base(context)
        {
            _context = context;
        }
    }
}