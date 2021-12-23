using System.Threading.Tasks;

namespace Pact.Sms;

/// <summary>
/// Abstraction for an SMS (and optionally Voice) delivery service
/// </summary>
public interface ISmsSender
{
    /// <summary>
    /// Indicates that the <see cref="SendVoiceAsync"/> call is supported
    /// </summary>
    bool SupportsVoice { get; }

    /// <summary>
    /// Sends an SMS to the provided number using the underlying delivery service
    /// </summary>
    /// <param name="number"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    Task SendSmsAsync(string number, string message);

    /// <summary>
    /// Negotiates a callback endpoint with a voice message service to communicate an audible message to a user
    /// The callback URL itself typically identifies/includes the content to be communicated when they arrive
    /// </summary>
    /// <param name="number"></param>
    /// <param name="callbackUrl"></param>
    /// <returns></returns>
    Task SendVoiceAsync(string number, string callbackUrl);
}