using System.Collections.Generic;
using System.Threading.Tasks;

namespace _01_SocialNetworkQuery.Contracts
{
    /// <summary>
    /// ready the queries of application we need 
    /// </summary>
    public interface IMessageQuery
    {
        /// <summary>
        /// load all chats between user with id=<paramref name="idUserA"/> and user with id=<see cref="idUserB"/>
        /// </summary>
        /// <param name="idUserA"></param>
        /// <param name="idUserB"></param>
        /// <returns>Return a json string of List<see cref="MessageViewModel"/>> or <see langword="null"/></returns>
        Task<string> LoadChatHistory(long idUserA, long idUserB);
    }
}