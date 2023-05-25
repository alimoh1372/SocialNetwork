using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using _01_SocialNetworkQuery.Contracts;
using SocialNetwork.Application.Contracts.MessageContracts;


namespace _01_SocialNetworkQuery.Implementations
{
    public class MessageQuery:IMessageQuery
    {
        private readonly IMessageApplication _messageApplication;

        public MessageQuery(IMessageApplication messageApplication)
        {
            _messageApplication = messageApplication;
        }

        public async Task<string> LoadChatHistory(long idUserA, long idUserB)
        {
           var result= await _messageApplication.LoadChatHistory(idUserA, idUserB);
           var json = JsonSerializer.Serialize(result);
           return json;

        }
    }
}