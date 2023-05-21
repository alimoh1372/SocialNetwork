using System.Collections.Generic;
using System.Threading.Tasks;
using _00_Framework.Application;
using SocialNetwork.Application.Contracts.MessageContracts;
using SocialNetwork.Domain.MessageAgg;

namespace SocialNetwork.Application
{
    public class MessageApplication:IMessageApplication
    {
        private readonly IMessageRepository _messageRepository;

        public MessageApplication(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public OperationResult Send(SendMessage command)
        {
            OperationResult result = new OperationResult();

            //check the message isn't from a user to himself
            if (command.FkToUserId == command.FkFromUserId)
                return result.Failed(ApplicationMessage.CantSelfRequest);

            Message message = new Message(command.FkFromUserId, command.FkToUserId, command.MessageContent);

            //Add to database
            _messageRepository.Create(message);


            _messageRepository.SaveChanges();
            //Todo:Send a notify to reciver person or reload that page if he is online

            return result.Succedded();

        }

        public OperationResult Edit(EditMessage command)
        {
            throw new System.NotImplementedException();
        }

        public OperationResult Like(long id)
        {
            throw new System.NotImplementedException();
        }

        public OperationResult Unlike(long id)
        {
            throw new System.NotImplementedException();
        }

        public OperationResult AsRead(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<MessageViewModel>> LoadChatHistory(long idUserA, long idUserB)
        {
            
            return await _messageRepository.LoadChatHistory(idUserA, idUserB);

        }

        public async Task<MessageViewModel> GetLatestMessage(long fromUserId, long toUserId)
        {
            return await _messageRepository.GetLatestMessageBy(fromUserId,toUserId);
        }
    }
}