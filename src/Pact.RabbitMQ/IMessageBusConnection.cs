using RabbitMQ.Client;

namespace Pact.RabbitMQ
{
    /// <summary>
    /// Defines the raw message bus connection handler
    /// </summary>
    public interface IMessageBusConnection
    {
        /// <summary>
        /// Gets the current channel to the server.
        /// </summary>
        /// <returns></returns>
        IModel GetModel();
    }
}