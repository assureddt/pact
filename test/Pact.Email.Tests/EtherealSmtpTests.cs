using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Pact.Email.Tests
{
    public class EtherealSmtpTests
    {
        /// <summary>
        /// Note: this will be skipped on CI, but will run locally. You will need to provide valid credentials in User Secrets
        /// via: https://ethereal.email/ in order for the test to pass
        /// </summary>
        /// <returns></returns>
        [SkippableFact]
        public async Task Smtp_Auth_OK()
        {
            Skip.If(IsCI);

            // arrange
            var services = new ServiceCollection();

            var config = new ConfigurationBuilder()
                .AddUserSecrets<EtherealSmtpTests>()
                .Build();

            var client = new SmtpClient();
            services.AddSingleton<ISmtpClient>(client);
            services.Configure<EmailSettings>(opts =>
            {
                opts.FromAddress = config["EtherealUsername"];
                opts.FromName = "Pact.Email.Tests";
                opts.SmtpPort = 587;
                opts.SmtpUri = "smtp.ethereal.email";
                opts.Username = config["EtherealUsername"];
                opts.Password = config["EtherealPassword"];
                opts.SmtpSslMode = SecureSocketOptions.StartTls;
            });
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSingleton<ILogger<EmailSender>>(new NullLogger<EmailSender>());
            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<IEmailSender>();

            // act
            await sender.SendEmailAsync("test@test.com", "test", "welcome to my test");
        }

        private static bool IsCI => Environment.GetEnvironmentVariable("CI") != null;
    }
}
