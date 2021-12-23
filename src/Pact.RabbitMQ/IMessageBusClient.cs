using RabbitMQ.Client;

namespace Pact.RabbitMQ;

/// <summary>
/// Defines the raw message bus client object.
/// </summary>
public interface IMessageBusClient
{
    IModel Channel { get; }
    MessageBusSettings Options { get; }

    /// <summary>
    /// Sends errors to the defined error exchange
    /// </summary>
    /// <param name="data"></param>
    /// <param name="originalKey"></param>
    void SendError(byte[] data, string originalKey);
}