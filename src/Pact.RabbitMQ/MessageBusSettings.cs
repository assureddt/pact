using System.Collections.Generic;

namespace Pact.RabbitMQ;

public class MessageBusSettings
{
    /// <summary>
    /// Address of RabbitMQ server
    /// </summary>
    public string Hostname { get; set; }

    /// <summary>
    /// Username to login to RabbitMQ
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Password to login to RabbitMQ
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Top level exchange to interact with
    /// </summary>
    public List<MessageBusExchange> Exchanges { get; set; }

    /// <summary>
    /// Error exchange to push failed messages too
    /// </summary>
    public string ErrorExchange { get; set; }
}

public class MessageBusExchange
{
    /// <summary>
    /// Name of Exchange
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// RabbitMQ type of exchange
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Set if exchange is durable
    /// </summary>
    public bool Durable { get; set; }

    /// <summary>
    /// Exchange is removed when it no longer has connections
    /// </summary>
    public bool AutoDelete { get; set; }
}