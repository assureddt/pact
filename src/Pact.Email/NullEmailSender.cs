using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Pact.Email.Helpers;

namespace Pact.Email
{
    public class NullEmailSender : IEmailSender
    {
        private readonly ILogger<NullEmailSender> _logger;

        public NullEmailSender(IOptions<EmailSettings> emailSettings, ILogger<NullEmailSender> logger)
        {
            Settings = emailSettings.Value;
            _logger = logger;
        }

        public EmailSettings Settings { get; set; }

        public List<MimeMessage> SentEmails { get; } = new List<MimeMessage>();

        public Task SendEmailAsync(string recipients, string subject, string message, params MimePart[] attachments)
        {
            return SendEmailAsync(recipients, subject, message,
                new MailboxAddress(Settings.FromName, Settings.FromAddress),
                attachments);
        }

        public Task SendEmailAsync(string recipients, string subject, string message, MailboxAddress from, params MimePart[] attachments)
        {
            return SendEmailAsync(recipients, subject, message,
                from,
                from,
                attachments);
        }

        public Task SendEmailAsync(string recipients, string subject, string message, MailboxAddress from, MailboxAddress sender, params MimePart[] attachments)
        {
            return Task.Run(() =>
            {
                foreach (var recipient in recipients.GetEmailAddresses())
                {
                    try
                    {
                        var emailMessage = new MimeMessage();

                        emailMessage.From.Add(from);
                        emailMessage.Sender = sender;

                        emailMessage.To.Add(!string.IsNullOrWhiteSpace(Settings.OverrideToAddress)
                            ? new MailboxAddress((string)null, Settings.OverrideToAddress)
                            : new MailboxAddress((string)null, recipient));

                        emailMessage.Subject = subject;

                        if (!string.IsNullOrWhiteSpace(Settings.OverrideToAddress))
                            emailMessage.Subject = $"*UAT* => {emailMessage.Subject} [To: {recipient}]";

                        BodyBuilder builder;
                        if (message.Contains("<head>") && message.Contains("</head>"))
                            builder = new BodyBuilder {HtmlBody = message};
                        else
                            builder = new BodyBuilder
                                {HtmlBody = $"<html>{DefaultHeader}<body>{message}</body></html>"};

                        foreach (var attachment in attachments)
                        {
                            builder.Attachments.Add(attachment);
                        }

                        emailMessage.Body = builder.ToMessageBody();

                        using (_logger.BeginScope(new Dictionary<string, object>
                        {
                            {"From", emailMessage.From.Select(x => x.ToString())},
                            {"Body", emailMessage.HtmlBody},
                            {"Attachments", emailMessage.Attachments.Select(x => (x as MimePart)?.FileName)}
                        }))
                        {
                            _logger.LogInformation("Null Smtp Sending => {Subject}, {To}",
                                emailMessage.Subject, recipient);
                        }

                        SentEmails.Add(emailMessage);
                    }
                    catch (Exception exc)
                    {
                        _logger.LogWarning(exc,"Smtp Send Error => {Subject}, {To}",
                            subject, recipient);
                    }
                }
            });
        }

        private const string DefaultHeader =
            "<head>" +
            "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>" +
            "<style type='text/css'>" +
            "body {"+
                "font-family: 'Open Sans', sans-serif, 'Helvetica Neue', Helvetica, Arial;" +
                "font-size: 10pt;" +
                "line-height: 1.65;" +
                "color: #000;"+
                "background-color: #fff;"+
            "}" +
            "</style>" +
            "</head>";
    }
}
