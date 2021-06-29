using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Pact.Email.Tests
{
    /// <summary>
    /// See: https://ethereal.email/
    /// Test creds below may no longer be valid, but no harm in trying
    /// </summary>
    public class EtherealSmtpTests
    {
        [Fact (Skip = "Ethereal Email account not intended for persistent testing")]
        public async Task Smtp_Auth_OK()
        {
            // arrange
            var services = new ServiceCollection();

            var client = new SmtpClient();
            services.AddSingleton<ISmtpClient>(client);
            services.Configure<EmailSettings>(opts =>
            {
                opts.FromAddress = "aliyah37@ethereal.email";
                opts.FromName = "Aliyah Schumm";
                opts.SmtpPort = 587;
                opts.SmtpUri = "smtp.ethereal.email";
                opts.Username = "aliyah37@ethereal.email";
                opts.Password = "JR12QjSYvREXbHP1xa";
                opts.SmtpSslMode = SecureSocketOptions.StartTls;
            });
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSingleton<ILogger<EmailSender>>(new NullLogger<EmailSender>());
            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<IEmailSender>();

            // act
            await sender.SendEmailAsync("test@test.com", "test", "welcome to my test");
        }
    }
}
