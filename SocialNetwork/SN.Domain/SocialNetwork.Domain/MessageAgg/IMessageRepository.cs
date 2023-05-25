using System.Collections.Generic;
using System.Threading.Tasks;
using _00_Framework.Domain;
using SocialNetwork.Application.Contracts.MessageContracts;


namespace SocialNetwork.Domain.MessageAgg
{
    /// <summary>
    /// To relate the entity with application layer and use the database
    /// </summary>
    public interface IMessageRepository:IBaseRepository<long,Message>
    {
        Task<List<MessageViewModel>> LoadChatHistory(long idUserA, long idUserB);
        Task<MessageViewModel> GetLatestMessageBy(long fromUserId, long toUserId);
        Task<EditMessage> GetEditMessage(long id);
    }
}