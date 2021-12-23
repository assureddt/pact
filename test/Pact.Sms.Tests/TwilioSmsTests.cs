using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Twilio.Clients;
using Twilio.Http;
using Xunit;

namespace Pact.Sms.Tests;

public class TwilioSmsTests
{
    [Fact]
    public async Task Sms_OK()
    {
        // arrange
        var twilioRest = new Mock<ITwilioRestClient>();
        var twilioHttp = new Mock<HttpClient>();
        twilioRest.Setup(m => m.HttpClient).Returns(twilioHttp.Object);
        twilioRest.Setup(m => m.RequestAsync(It.IsAny<Request>())).ReturnsAsync(new Response(HttpStatusCode.OK, ""));

        var options = new OptionsWrapper<SmsSettings>(new SmsSettings
        {
            From = "+447974000000",
            Username = "Test",
            Password = "Test2"
        });
        var svc = new TwilioSmsSender(options, new NullLogger<TwilioSmsSender>(), twilioRest.Object);

        // act
        await svc.SendSmsAsync("+447974000000", "howdy");

        // assert
        // we can't actually assert that much meaningful actually happens, just satisfy ourselves that the twilio bits are called
        twilioRest.Verify(m => m.AccountSid);
        twilioRest.Verify(m => m.RequestAsync(It.IsAny<Request>()));
        twilioRest.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Voice_OK()
    {
        // arrange
        var twilioRest = new Mock<ITwilioRestClient>();
        var twilioHttp = new Mock<HttpClient>();
        twilioRest.Setup(m => m.HttpClient).Returns(twilioHttp.Object);
        twilioRest.Setup(m => m.RequestAsync(It.IsAny<Request>())).ReturnsAsync(new Response(HttpStatusCode.OK, ""));

        var options = new OptionsWrapper<SmsSettings>(new SmsSettings
        {
            From = "+447974000000",
            Username = "Test",
            Password = "Test2"
        });
        var svc = new TwilioSmsSender(options, new NullLogger<TwilioSmsSender>(), twilioRest.Object);

        // act
        await svc.SendVoiceAsync("+447974000000", "https://test.com/api/voice");

        // assert
        // we can't actually assert that much meaningful actually happens, just satisfy ourselves that the twilio bits are called
        svc.SupportsVoice.ShouldBeTrue();
        twilioRest.Verify(m => m.AccountSid);
        twilioRest.Verify(m => m.RequestAsync(It.IsAny<Request>()));
        twilioRest.VerifyNoOtherCalls();
    }
}