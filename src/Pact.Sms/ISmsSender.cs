using System.Threading.Tasks;

namespace Pact.Sms
{
    public interface ISmsSender
    {
        SmsSettings Settings { get; set; }
        bool SupportsVoice { get; }

        Task SendSmsAsync(string number, string message);
        Task SendVoiceAsync(string number, string callbackUrl);
    }
}
