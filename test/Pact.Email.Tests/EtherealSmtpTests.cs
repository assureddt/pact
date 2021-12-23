using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Pact.Email.Tests;

public class EtherealSmtpTests
{
    private readonly EmailSettings _settings;

    public EtherealSmtpTests()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<EtherealSmtpTests>(true)
            .Build();

        _settings = new EmailSettings
        {
            FromAddress = config["EtherealUsername"],
            FromName = "Pact.Email.Tests",
            SmtpPort = 587,
            SmtpUri = "smtp.ethereal.email",
            Username = config["EtherealUsername"],
            Password = config["EtherealPassword"],
            SmtpSslMode = SecureSocketOptions.StartTls
        };
    }

    /// <summary>
    /// Note: this will be skipped on CI, but will run locally. You will need to provide valid credentials in User Secrets
    /// via: https://ethereal.email/ in order for the test to pass
    /// </summary>
    /// <returns></returns>
    [SkippableFact]
    public async Task Smtp_Auth_OK()
    {
        Skip.If(IsCI, "Not tested on CI");
        Skip.If(string.IsNullOrWhiteSpace(_settings.FromAddress), "No credentials in User Secrets");
        Skip.If(string.IsNullOrWhiteSpace(_settings.Username), "No credentials in User Secrets");
        Skip.If(string.IsNullOrWhiteSpace(_settings.Password), "No credentials in User Secrets");

        // arrange
        var services = new ServiceCollection();

        var client = new SmtpClient();
        services.AddSingleton<ISmtpClient>(client);
        services.AddSingleton<IOptions<EmailSettings>>(new OptionsWrapper<EmailSettings>(_settings));
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddSingleton<ILogger<EmailSender>>(new NullLogger<EmailSender>());
        var provider = services.BuildServiceProvider();

        var sender = provider.GetService<IEmailSender>();

        // act
        await sender.SendEmailAsync("test@test.com", "test", "welcome to my test");
    }

    private static bool IsCI => Environment.GetEnvironmentVariable("CI") != null;
}