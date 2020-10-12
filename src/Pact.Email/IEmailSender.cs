using System.Threading.Tasks;
using MimeKit;

namespace Pact.Email
{
    public interface IEmailSender
    {
        EmailSettings Settings { get; set; }

        Task SendEmailAsync(string recipients, string subject, string message, params MimePart[] attachments);
        Task SendEmailAsync(string recipients, string subject, string message, MailboxAddress from, params MimePart[] attachments);
        Task SendEmailAsync(string recipients, string subject, string message, MailboxAddress from, MailboxAddress sender, params MimePart[] attachments);
    }
}
