using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pact.Core.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Pact.RabbitMQ
{
    /// <summary>
    /// A basic class used to listen for events
    /// </summary>
    /// <typeparam name="T">Type of object received in messages</typeparam>
    public abstract class MessageBusListener<T> : IMessageBusListener where T : class
    {
        protected MessageBusListener(ILoggerFactory loggerFactory, IServiceProvider services)
        {
            _services = services;
            Logger = loggerFactory.CreateLogger(GetType().Name);
        }

        private readonly IServiceProvider _services;
        public readonly ILogger Logger;

        public abstract string Name { get; }
        public abstract string Key { get; }
        public abstract string Exchange { get; }

        /// <summary>
        /// Message processing handler (non-RPC)
        /// </summary>
        /// <param name="services">A scoped service provider for this message</param>
        /// <param name="message">The message object</param>
        /// <returns>Task completion result</returns>
        public abstract Task ProcessMessage(IServiceProvider services, T message);

        /// <summary>
        /// Set up the message bus listener
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public Task Setup(IMessageBusClient client)
        {
            try
            {
                //Create a disposable queue
                var queueName = client.Channel.QueueDeclare(Name, true, false, false).QueueName;

                client.Channel.BasicQos(0, 1, false);

                //Bind it to the exchange
                client.Channel.QueueBind(queueName, Exchange, Key);

                var consumer = new EventingBasicConsumer(client.Channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        using var scope = _services.CreateScope();
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                        var item = message.FromJson<T>();

                        await ProcessMessage(scope.ServiceProvider, item);

                        client.Channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception exc)
                    {
                        var body = ea.Body.ToArray();

                        Logger.LogTrace(exc, "RabbitMQ Message Failed => {name} => {message}", Name, Encoding.UTF8.GetString(body));

                        client.Channel.BasicNack(ea.DeliveryTag, false, false);

                        client.SendError(body, ea.RoutingKey);
                    }
                };
                client.Channel.BasicConsume(queueName, false, consumer);
            }
            catch (Exception exc)
            {
                Logger.LogError(exc, "Error setting up message bus listener => {name}", Name);
            }

            return Task.CompletedTask;
        }
    }
}