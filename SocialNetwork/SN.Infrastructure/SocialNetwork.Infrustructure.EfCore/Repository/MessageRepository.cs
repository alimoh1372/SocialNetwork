using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _00_Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.Contracts.MessageContracts;
using SocialNetwork.Domain.MessageAgg;

namespace SocialNetwork.Infrastructure.EfCore.Repository
{
    public class MessageRepository : BaseRepository<long, Message>, IMessageRepository
    {
        private readonly SocialNetworkContext _context;
        public MessageRepository(SocialNetworkContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<MessageViewModel>> LoadChatHistory(long idUserA, long idUserB)
        {
            return await _context.Messages
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .Select(x => new MessageViewModel
                {
                    Id = x.Id,
                    CreationDate = x.CreationDate,
                    FkFromUserId = x.FkFromUserId,
                    SenderFullName = x.FromUser.Name + " " + x.FromUser.LastName,
                    FkToUserId = x.FkToUserId,
                    ReceiverFullName = x.ToUser.Name + " " + x.ToUser.LastName,
                    MessageContent = x.MessageContent
                })
                .Where(x=>(x.FkFromUserId== idUserA && x.FkToUserId==idUserB)
                          ||(x.FkFromUserId==idUserB && x.FkToUserId==idUserA))
                .ToListAsync();
        }

        public async Task<MessageViewModel> GetLatestMessageBy(long fromUserId, long toUserId)
        {
           return await _context.Messages
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .Select(x => new MessageViewModel
                {
                    Id = x.Id,
                    CreationDate = x.CreationDate,
                    FkFromUserId = x.FkFromUserId,
                    SenderFullName = x.FromUser.Name + " " + x.FromUser.LastName,
                    FkToUserId = x.FkToUserId,
                    ReceiverFullName = x.ToUser.Name + " " + x.ToUser.LastName,
                    MessageContent = x.MessageContent
                }).
                OrderBy(x=>x.Id).
                LastOrDefaultAsync(x => x.FkFromUserId == fromUserId && x.FkToUserId == toUserId);
        }
    }
}