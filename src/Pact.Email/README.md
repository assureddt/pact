# Pact.Email ✉
Provides an [EmailSender](./EmailSender.cs) service implementation to deliver emails either via SMTP or a file-storage maildrop path (common with IIS6 SMTP configured with a smart-host).
The implementation supports attachments and also the option of an override delivery address & whitelist (useful for testing environments - allows you to redirect all outbound email to a specific address, except for emails already destined for specified testing addresses) via [EmailSettings](./EmailSettings.cs).

DI service extensions for both variations can be found in [ServiceCollectionExtensions](./ServiceCollectionExtensions.cs).

Configuring an SMTP Maildrop folder-based delivery would look as follows, in Startup.Configure:

```c#
services.AddTransient<IEmailSender, EmailSender>();
services.AddTransient<IMaildropProvider, MaildropProvider>();
services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
```

Alternatively, you could replace the IMaildropProvider implementation with an ISmtpClient one to use an SMTP service directly.
Providing neither will just force the sender to act as a "Null" delivery service (where it'll pretend to send the email and log it, but actually do nothing - for development scenarios).

An example usage of the SendEmailAsync method with an attachment, would then be:

```c#
var attachment = new MimePart
{
    Content = new MimeContent(stream),
    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
    ContentTransferEncoding = ContentEncoding.Base64,
    FileName = "Attachment.pdf"
};
  
await _emailSender.SendEmailAsync("fred.smith@foo.bar; george.davis@foo.bar", "Hello World!", "Howdy", attachment);
```

Now supports SMTP AUTH (for services where the mail server requires authentication e.g. M365).
Simply provide an associated Username & Password in the EmailSettings configuration to enable that authentication step in the connection to the SMTP server.
You will also likely need to be using the `SmtpSslMode = SecureSocketOptions.StartTls` option to support this.

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Email-Index)
