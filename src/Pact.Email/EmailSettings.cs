using MailKit.Security;

namespace Pact.Email;

public class EmailSettings
{
    public string FromName { get; set; }
    public string FromAddress { get; set; }
    public string MaildropPath { get; set; }
    public string SmtpUri { get; set; }
    public int SmtpPort { get; set; }
    public SecureSocketOptions SmtpSslMode { get; set; } = SecureSocketOptions.Auto;
    public string OverrideToAddress { get; set; }
    public string[] OverrideWhitelist { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}