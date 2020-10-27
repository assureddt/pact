using System.Threading.Tasks;

namespace Pact.RabbitMQ
{
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