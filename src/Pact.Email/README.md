# Pact.Email ✉
Provides an [EmailSender](Pact.Email/EmailSender.cs) service implementation to deliver emails either via SMTP or a file-storage maildrop path (common with IIS6 SMTP configured with a smart-host).
The implementation supports attachments and also the option of an override delivery address & whitelist (useful for testing environments - allows you to redirect all outbound email to a specific address, except for emails already destined for specified testing addresses) via [EmailSettings](Pact.Email/EmailSettings.cs).

DI service extensions for both variations can be found in [ServiceCollectionExtensions](Pact.Email/ServiceCollectionExtensions.cs).

An example usage of the SendEmailAsync method follows:
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
The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Email-Index)
