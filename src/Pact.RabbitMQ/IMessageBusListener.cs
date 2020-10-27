using System.Threading.Tasks;

namespace Pact.RabbitMQ
{
    /// <summary>
    /// Defines handler for listening to messages
    /// </summary>
    public interface IMessageBusListener
    {
        string Name { get; }

        /// <summary>
        /// Set up the message bus listener
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        Task Setup(IMessageBusClient client);
    }
}