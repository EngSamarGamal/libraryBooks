using Microsoft.AspNetCore.SignalR;

namespace Shopify.PL.Helpers
{
    public class OrderHub:Hub
    {
        public async Task SendOrderNotification(string orderData)
        {
            await Clients.All.SendAsync("ReceiveOrderNotification", orderData);
        }
    }
}
