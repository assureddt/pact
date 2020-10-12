using System.Threading.Tasks;

namespace Pact.MessageBus
{
    public interface IMessageBusSender
    {
        /// <summary>
        /// Sends the item to the key and exchange combination 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="exchange"></param>
        /// <param name="key"></param>
        void Send(object item, string exchange, string key);

        /// <summary>
        /// Sends the item to the key and exchange combination and waits for a response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="exchange"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> SendRPCAsync<T>(object item, string exchange, string key) where T : class;
    }
}