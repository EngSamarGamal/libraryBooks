using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace Shopify.BLL.Services
{
    public interface IMessageService
    {
        Task<MessageResource> SendAsync(string MobileNumber, string Body);
    }
}
