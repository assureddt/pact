using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MimeKit;
using Moq;
using Shouldly;
using Xunit;

namespace Pact.Email.Tests
{
    public class EmailSenderTests
    {
        [Fact]
        public async Task Maildrop_OK()
        {
            // arrange
            var services = new ServiceCollection();
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            
            var drop = new Mock<IMaildropProvider>();
            drop.Setup(m => m.GetStreamWriter()).Returns(sw);
            services.AddSingleton(drop.Object);
            services.Configure<EmailSettings>(opts =>
            {
                opts.FromAddress = "origin@test.com";
                opts.FromName = "Origin";
            });
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSingleton<ILogger<EmailSender>>(new NullLogger<EmailSender>());
            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<IEmailSender>();

            // act
            await sender.SendEmailAsync("test@test.com", "test", "welcome to my test");

            // assert
            drop.Verify(m => m.GetStreamWriter());
            drop.VerifyNoOtherCalls();

            var text = Encoding.UTF8.GetString(ms.ToArray());
            text.ShouldContain("welcome to my test");
        }

        [Fact]
        public async Task Smtp_OK()
        {
            // arrange
            var services = new ServiceCollection();

            var client = new Mock<ISmtpClient>();
            services.AddSingleton(client.Object);
            services.Configure<EmailSettings>(opts =>
            {
                opts.FromAddress = "origin@test.com";
                opts.FromName = "Origin";
                opts.SmtpPort = 20;
                opts.SmtpUri = "127.0.0.1";
            });
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSingleton<ILogger<EmailSender>>(new NullLogger<EmailSender>());
            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<IEmailSender>();

            // act
            await sender.SendEmailAsync("test@test.com", "test", "welcome to my test");

            // assert
            client.Verify(m => m.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SecureSocketOptions>(), It.IsAny<CancellationToken>()));
            client.Verify(m => m.SendAsync(It.Is<MimeMessage>(x => x.HtmlBody.Contains("welcome to my test")),
                new CancellationToken(), null));
            client.Verify(m => m.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()));

            client.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Smtp_Auth_OK()
        {
            // arrange
            var services = new ServiceCollection();

            var client = new Mock<ISmtpClient>();
            services.AddSingleton(client.Object);
            var junkCreds = Guid.NewGuid().ToString();
            services.Configure<EmailSettings>(opts =>
            {
                opts.FromAddress = "origin@test.com";
                opts.FromName = "Origin";
                opts.SmtpPort = 20;
                opts.SmtpUri = "127.0.0.1";
                opts.Username = "test";
                opts.Password = junkCreds;
                opts.SmtpSslMode = SecureSocketOptions.StartTls;
            });
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSingleton<ILogger<EmailSender>>(new NullLogger<EmailSender>());
            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<IEmailSender>();

            // act
            await sender.SendEmailAsync("test@test.com", "test", "welcome to my test");

            // assert
            client.Verify(m => m.ConnectAsync("127.0.0.1", 20, SecureSocketOptions.StartTls, It.IsAny<CancellationToken>()));
            client.Verify(m => m.AuthenticateAsync("test", junkCreds, It.IsAny<CancellationToken>()));
            client.Verify(m => m.SendAsync(It.Is<MimeMessage>(x => x.HtmlBody.Contains("welcome to my test")),
                new CancellationToken(), null));
            client.Verify(m => m.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()));

            client.VerifyNoOtherCalls();
        }
    }
}
