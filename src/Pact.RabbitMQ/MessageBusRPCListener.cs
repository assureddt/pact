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
    public abstract class MessageBusRPCListener<T1, T2> : IMessageBusListener where T1 : class where T2 : class
    {
        protected MessageBusRPCListener(ILoggerFactory loggerFactory, IServiceProvider services)
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
        /// Message processing handler (RPC)
        /// </summary>
        /// <param name="services">A scoped service provider for this message</param>
        /// <param name="message">The message object</param>
        /// <returns>The result object of the message call</returns>
        public abstract Task<T2> ProcessMessage(IServiceProvider services, T1 message);

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
                        using (var scope = _services.CreateScope())
                        {
                            var messageJson = Encoding.UTF8.GetString(ea.Body.ToArray());

                            var message = messageJson.FromJson<T1>();

                            var response = await ProcessMessage(scope.ServiceProvider, message);

                            var responseJson = response.ToJson();

                            var responseData = Encoding.UTF8.GetBytes(responseJson);

                            //Send result
                            var replyProps = client.Channel.CreateBasicProperties();
                            replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                            client.Channel.BasicPublish("", ea.BasicProperties.ReplyTo, replyProps, responseData);

                            client.Channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                    catch (Exception exc)
                    {
                        var body = ea.Body.ToArray();

                        Logger.LogError(exc, "RabbitMQ Message Failed => {name} => {message}", Name, Encoding.UTF8.GetString(body));

                        client.Channel.BasicNack(ea.DeliveryTag, false, false);

                        client.SendError(body, ea.RoutingKey);
                    }
                };
                client.Channel.BasicConsume(queueName, false, consumer);
            }
            catch (Exception exc)
            {
                Logger.LogError(exc, "Error setting up message bus listener (rpc) {name}", Name);
            }

            return Task.CompletedTask;
        }
    }
}