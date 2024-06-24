using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Shopify.BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly TwilioSetting _twilio;
        public MessageService(IOptions<TwilioSetting> setting)
        {
            _twilio=setting.Value;
        }

        public async Task<MessageResource> SendAsync(string MobileNumber, string Body)
        {
            TwilioClient.Init(_twilio.AccountSid, _twilio.AuthToken);
            var result = await MessageResource.CreateAsync(
                from: new Twilio.Types.PhoneNumber($"whatsapp:{_twilio.TwilioPhoneNumber}"),
                body: Body,
                to: new Twilio.Types.PhoneNumber($"whatsapp:{MobileNumber}"));

            return result;
        }
    }
}
