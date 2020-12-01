# Pact.Sms 📲
Provides an SMS Sender [TwilioSmsSender](./TwilioSmsSender.cs) service implementation to deliver SMS messages either via the Twilio provider.
The implementation also supports a callback-based Voice message, which requires the use of the Twilio Voice SDK on a designated MVC Controller. Account credentials are provided via [SmsSettings](./SmsSettings.cs).


An example usage of the SendSmsAsync method follows:
```c#
await _smsSender.SendSmsAsync("0123456789", "Hello World!");
```

An example usage of the SendVoiceAsync method follows (the callback endpoint would typically take a parameter to customize the response returned to Twilio based on the original request):
```c#
await _smsSender.SendVoiceAsync("0123456789", "https://foo.bar/voice/message?ref=abcdefgh");
```

As an aside, the following code can be a good reference for validating and cleaning up the format of a phone number (uses other Twilio SDK resources)
```c#
string cleanedFullNumber = null;
try
{
    var validationResult = await PhoneNumberResource.FetchAsync(new PhoneNumber(Input.PhoneNumber), Input.CountryCode, client: _twilio);

    if (validationResult.PhoneNumber != null)
    {
        cleanedFullNumber = validationResult.PhoneNumber.ToString();
    }
}
catch (ApiException exc)
{
    if (exc.Code == 20008)
    {
        // test account allowance
        cleanedFullNumber = Input.PhoneNumber;
        Logger.LogWarning(exc, "Number validation skipped (Test) => {Number}, {CountryCode]", Input.PhoneNumber, Input.CountryCode);
    }
    else
    {
        Logger.LogWarning(exc, "Exception when validating number with Twilio => {Number}, {CountryCode]", Input.PhoneNumber, Input.CountryCode);
    }
}
```

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-Sms-Index)
