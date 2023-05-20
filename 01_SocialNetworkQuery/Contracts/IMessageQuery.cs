using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetwork.Application.Contracts.MessageContracts;

namespace _01_SocialNetworkQuery.Contracts
{
    public interface IMessageQuery
    {
        Task<string> LoadChatHistory(long idUserA, long idUserB);
    }
}