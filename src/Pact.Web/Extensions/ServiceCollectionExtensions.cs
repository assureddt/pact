using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pact.Web.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Retrieves a strongly-typed named section from the configuration, but also registers it with Configure internally.
    /// This override assumes the name of the configuration section matches the type name of the configuration object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>A strongly-typed configuration object</returns>
    public static T GetConfigurationSection<T>(this IServiceCollection services, IConfiguration configuration) where T : class
    {
        return services.GetConfigurationSection<T>(configuration, typeof(T).Name);
    }

    /// <summary>
    /// Retrieves a strongly-typed named section from the configuration, but also registers it with Configure internally.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="key">The name of the configuration section to extract</param>
    /// <returns>A strongly-typed configuration object</returns>
    public static T GetConfigurationSection<T>(this IServiceCollection services, IConfiguration configuration, string key) where T: class
    {
        return services.GetConfigurationSection<T>(configuration, key, out _);
    }

    /// <summary>
    /// Retrieves a strongly-typed named section from the configuration, but also registers it with Configure internally.
    /// This override assumes the name of the configuration section matches the type name of the configuration object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="section">Optional output of the configuration section itself (may be useful for binding)</param>
    /// <returns>A strongly-typed configuration object</returns>
    public static T GetConfigurationSection<T>(this IServiceCollection services, IConfiguration configuration, out IConfigurationSection section) where T : class
    {
        return services.GetConfigurationSection<T>(configuration, typeof(T).Name, out section);
    }

    /// <summary>
    /// Retrieves a strongly-typed named section from the configuration, but also registers it with Configure internally.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="key">The name of the configuration section to extract</param>
    /// <param name="section">Optional output of the configuration section itself (may be useful for binding)</param>
    /// <returns>A strongly-typed configuration object</returns>
    public static T GetConfigurationSection<T>(this IServiceCollection services, IConfiguration configuration, string key, out IConfigurationSection section) where T: class
    {
        section = configuration.GetSection(key);
        services.Configure<T>(section);

        return section.Get<T>();
    }
}