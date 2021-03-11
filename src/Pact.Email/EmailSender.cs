using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Pact.Core.Extensions;

namespace Pact.Email
{
    /// <inheritdoc/>
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger, IServiceProvider serviceProvider)
        {
            _settings = emailSettings.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public Task SendEmailAsync(string recipients, string subject, string message, params MimePart[] attachments)
        {
            return SendEmailAsync(recipients, subject, message,
                new MailboxAddress(_settings.FromName, _settings.FromAddress),
                attachments);
        }

        /// <inheritdoc/>
        public Task SendEmailAsync(string recipients, string subject, string message, MailboxAddress from, params MimePart[] attachments)
        {
            return SendEmailAsync(recipients, subject, message,
                from,
                from,
                attachments);
        }

        /// <inheritdoc/>
        public async Task SendEmailAsync(string recipients, string subject, string message, MailboxAddress from, MailboxAddress sender, params MimePart[] attachments)
        {
            var client = _serviceProvider.GetService<ISmtpClient>();
            var maildrop = _serviceProvider.GetService<IMaildropProvider>();

            try
            {
                if (client != null)
                {
                    await client
                        .ConnectAsync(_settings.SmtpUri, _settings.SmtpPort, _settings.SmtpSslMode)
                        .ConfigureAwait(false);
                }

                foreach (var recipient in recipients.GetEmailAddresses())
                {
                    try
                    {
                        var emailMessage = new MimeMessage();

                        emailMessage.From.Add(from);
                        emailMessage.Sender = sender;

                        var redirection = !string.IsNullOrWhiteSpace(_settings.OverrideToAddress);
                        var recipientIsWhitelisted = _settings.OverrideWhitelist != null && _settings.OverrideWhitelist.Contains(recipient);

                        emailMessage.To.Add(redirection && !recipientIsWhitelisted
                            ? new MailboxAddress((string)null, _settings.OverrideToAddress)
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
                                _logger.LogInformation("Smtp Sending (Smtp) => {Subject}, {To}", emailMessage.Subject,
                                    recipient);

                                await client.SendAsync(emailMessage).ConfigureAwait(false);
                            }
                            else if (maildrop != null)
                            {
                                using var streamWriter = maildrop.GetStreamWriter();

                                _logger.LogInformation("Smtp Sending (File) => {Subject}, {To}", emailMessage.Subject, recipient);

                                await emailMessage.WriteToAsync(streamWriter.BaseStream).ConfigureAwait(false);
                            }
                            else
                            {
                                _logger.LogInformation("Smtp Sending (Null) => {Subject}, {To}", emailMessage.Subject, recipient);
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
