namespace Pact.RabbitMQ;

/// <summary>
/// Defines handler for sending messages
/// </summary>
public interface IMessageBusSender
{
    /// <summary>
    /// Sends a basic fire and forget message
    /// </summary>
    /// <param name="item">object to be sent</param>
    /// <param name="exchange">the bucket to send it too</param>
    /// <param name="key">the message routing key</param>
    void Send(object item, string exchange, string key);

    /// <summary>
    /// Sends a message and waits for a response
    /// </summary>
    /// <typeparam name="T">Expected return type</typeparam>
    /// <param name="item">object to be sent</param>
    /// <param name="exchange">the bucket to send it too</param>
    /// <param name="key">the message routing key</param>
    /// <returns>instance of T</returns>
    Task<T> SendRPCAsync<T>(object item, string exchange, string key) where T : class;
}