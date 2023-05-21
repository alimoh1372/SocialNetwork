using System.Collections.Generic;
using System.Threading.Tasks;
using _00_Framework.Application;

namespace SocialNetwork.Application.Contracts.MessageContracts
{
    public interface IMessageApplication
    {
        /// <summary>
        /// Send A message From UserA To UserB
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        OperationResult Send(SendMessage command);

        /// <summary>
        /// To edit message by user A(Sender)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        OperationResult Edit(EditMessage command);

        /// <summary>
        /// Like the message by UserB(reciever)
        /// </summary>
        /// <param name="id">id of message</param>
        /// <returns></returns>
        OperationResult Like(long id);

        /// <summary>
        /// Unlike the liked message by UserB(reciever)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OperationResult Unlike(long id);
        /// <summary>
        /// Show that the user B is read the message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OperationResult AsRead(long id);

        Task<List<MessageViewModel>> LoadChatHistory(long idUserA, long idUserB);
        Task<MessageViewModel> GetLatestMessage(long fromUserId, long toUserId);
    }
}