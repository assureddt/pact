using Microsoft.Extensions.Options;

namespace Pact.Email;

/// <inheritdoc/>
public class MaildropProvider : IMaildropProvider
{
    private readonly IOptions<EmailSettings> _settings;

    public MaildropProvider(IOptions<EmailSettings> settings)
    {
        _settings = settings;
    }

    public StreamWriter GetStreamWriter()
    {
        var path = Path.Combine(_settings.Value.MaildropPath, Guid.NewGuid().ToString("D") + ".eml");

        return File.CreateText(path);
    }
}