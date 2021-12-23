using MimeKit;

namespace Pact.Email;

/// <summary>
/// Provides sending functionality
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Compile and send an email using whichever transport service has been registered (or just log if none)
    /// </summary>
    /// <param name="recipients">Comma-separated list of recipients</param>
    /// <param name="subject">The Subject of the email</param>
    /// <param name="message">The Body of the email (HTML)</param>
    /// <param name="attachments">Any files to attach</param>
    /// <returns></returns>
    Task SendEmailAsync(string recipients, string subject, string message, params MimePart[] attachments);
    /// <summary>
    /// Compile and send an email using whichever transport service has been registered (or just log if none)
    /// </summary>
    /// <param name="recipients"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <param name="from"></param>
    /// <param name="attachments"></param>
    /// <returns></returns>
    Task SendEmailAsync(string recipients, string subject, string message, MailboxAddress from, params MimePart[] attachments);
    /// <summary>
    /// Compile and send an email using whichever transport service has been registered (or just log if none)
    /// </summary>
    /// <param name="recipients"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <param name="from"></param>
    /// <param name="sender"></param>
    /// <param name="attachments"></param>
    /// <returns></returns>
    Task SendEmailAsync(string recipients, string subject, string message, MailboxAddress from, MailboxAddress sender, params MimePart[] attachments);
}