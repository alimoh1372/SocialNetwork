using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
    }

    public class EditMessage
    {
        public long Id { get; set; }

        [DisplayName("Message text")]
        [Required]
        public string MessageContent { get; private set; }
    }

    public class SendMessage        
    {
        [DisplayName("Message from")]
        [Range(1,long.MaxValue)]
        public long FkFromUserId { get; private set; }

        [DisplayName("Message to")]
        [Range(1, long.MaxValue)]
        public long FkToUserId { get; private set; }

        [DisplayName("Message text")]
        [Required]
        public string MessageContent { get; private set; }

    }
}