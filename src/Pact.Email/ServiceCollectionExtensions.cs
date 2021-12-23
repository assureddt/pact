using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;

namespace Pact.Email;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMaildropEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IMaildropProvider, MaildropProvider>();

        return services;
    }

    public static IServiceCollection AddSmtpEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<ISmtpClient, SmtpClient>();

        return services;
    }
}