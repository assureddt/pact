namespace Pact.Email
{
    public class EmailSettings
    {
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string MaildropPath { get; set; }
        public string SmtpUri { get; set; }
        public int SmtpPort { get; set; }
        public string OverrideToAddress { get; set; }
        public string[] OverrideWhitelist { get; set; }
    }
}
