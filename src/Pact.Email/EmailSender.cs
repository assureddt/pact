using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Pact.Email.Helpers;

namespace Pact.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
        {
            Settings = emailSettings.Value;
            _logger = logger;
        }

        public EmailSettings Settings { get; set; }

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

        public async Task SendEmailAsync(string recipients, string subject, string message, MailboxAddress from, MailboxAddress sender, params MimePart[] attachments)
        {
            var useClient = string.IsNullOrWhiteSpace(Settings.MaildropPath);

            using var client = useClient ? new SmtpClient() : null;
            try
            {
                if (client != null)
                {
                    await client
                        .ConnectAsync(Settings.SmtpUri, Settings.SmtpPort, SecureSocketOptions.None)
                        .ConfigureAwait(false);
                }

                foreach (var recipient in recipients.GetEmailAddresses())
                {
                    try
                    {
                        var emailMessage = new MimeMessage();

                        emailMessage.From.Add(from);
                        emailMessage.Sender = sender;

                        var redirection = !string.IsNullOrWhiteSpace(Settings.OverrideToAddress);
                        var recipientIsWhitelisted = Settings.OverrideWhitelist != null && Settings.OverrideWhitelist.Contains(recipient);

                        emailMessage.To.Add(redirection && !recipientIsWhitelisted
                            ? new MailboxAddress((string)null, Settings.OverrideToAddress)
                            : new MailboxAddress((string)null, recipient));

                        emailMessage.Subject = subject;

                        if (redirection)
                            emailMessage.Subject = $"*UAT* => {emailMessage.Subject} [To: {recipient}]";

                        BodyBuilder builder;
                        if (message.Contains("<head>") && message.Contains("</head>"))
                            builder = new BodyBuilder {HtmlBody = message};
                        else
                            builder = new BodyBuilder
                                {HtmlBody = $"<html>{DefaultHeader}<body>{message}</body></html>"};

                        foreach (var attachment in attachments)
                        {
                            if (attachment.ContentDisposition.Parameters.TryGetValue("filename", out Parameter param))
                                param.EncodingMethod = ParameterEncodingMethod.Rfc2047;

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
                            if (client != null)
                            {
                                _logger.LogInformation("Smtp Sending => {Subject}, {To}", emailMessage.Subject,
                                    recipient);

                                await client.SendAsync(emailMessage).ConfigureAwait(false);
                            }
                            else
                            {
                                var path = Path.Combine(Settings.MaildropPath,
                                    Guid.NewGuid().ToString("D") + ".eml");

                                using var data = File.CreateText(path);

                                _logger.LogInformation("Smtp Sending => {Subject}, {To} ({FilePath})",
                                    emailMessage.Subject, recipient, path);

                                await emailMessage.WriteToAsync(data.BaseStream).ConfigureAwait(false);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        _logger.LogWarning(exc,"Smtp Send Error => {Subject}, {To}",
                            subject, recipient);
                    }
                }
            }

            finally
            {
                if (client != null)
                    await client.DisconnectAsync(true).ConfigureAwait(false);
            }
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
