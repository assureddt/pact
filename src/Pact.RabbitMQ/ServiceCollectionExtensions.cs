using Microsoft.Extensions.DependencyInjection;

namespace Pact.RabbitMQ
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Sets up message bus service with required dependencies
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessageBusService(this IServiceCollection services)
        {
            services.AddMessageBusClient();
            services.AddHostedService<MessageBusService>();
            return services;
        }

        /// <summary>
        /// Sets up message bus client with required dependencies
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessageBusClient(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBusConnection, MessageBusConnection>();
            services.AddTransient<IMessageBusClient, MessageBusClient>();
            services.AddTransient<IMessageBusSender, MessageBusSender>();
            return services;
        }
    }
}
