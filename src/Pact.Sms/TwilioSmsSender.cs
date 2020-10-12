﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Pact.Sms
{
    public class TwilioSmsSender : ISmsSender
    {
        private readonly ILogger<TwilioSmsSender> _logger;
        private readonly TwilioRestClient _twilio;

        public TwilioSmsSender(IOptions<SmsSettings> smsSettings,
            ILogger<TwilioSmsSender> logger,
            TwilioRestClient twilio)
        {
            Settings = smsSettings.Value;
            _twilio = twilio;
            _logger = logger;
        }

        public SmsSettings Settings { get; set; }
        public bool SupportsVoice => true;

        public async Task SendVoiceAsync(string number, string callbackUrl)
        {
            _logger.LogInformation("> Voice Sending => {Number}, {CallbackUrl}", number, callbackUrl);

            var to = new PhoneNumber(number);
            var from = new PhoneNumber(Settings.From);

            var call = await CallResource.CreateAsync(to, from, url: new Uri(callbackUrl), client: _twilio).ConfigureAwait(false);

            _logger.LogInformation(">> Voice Sent => {Number}, {CallbackUrl}, {Sid}", number, callbackUrl, call.Sid);
        }

        public async Task SendSmsAsync(string number, string message)
        {
            _logger.LogInformation("> Sms Sending => {Number}, {Message}", number, message);

            var to = new PhoneNumber(number);
            var from = new PhoneNumber(Settings.From);

            await MessageResource.CreateAsync(to, from: from, body: message, client: _twilio).ConfigureAwait(false);

            _logger.LogInformation(">> Sms Sent => {Number}, {Message}", number, message);
        }
    }
}