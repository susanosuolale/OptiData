using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OptiData.Application.Interfaces;
using OptiData.Presentation.Hubs;

namespace OptiData.Presentation.Services
{
    public class SignalRNotificationService : INotificationService
    {
        // c# connects the server to the browser via the URL the browser calls
        // <NotificationHub> specifies the name of the list c# then stores in memory
        // of all the browsers that connect to the server through that URL
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        
        public async Task SendPurchaseNotificationAsync(string message)
        {
            // targets every single web browser (under the hub specified) that currently has the OptiData webpage open on their screen
            //  forces a message down the live connection directly into those browsers
            // "ReceivePurchaseNotification" is the name of the event JS(browser) must look out for
            // when listening for the message(forced down the live connection) on the client side
            await _hubContext.Clients.All.SendAsync("ReceivePurchaseNotification", message);
        }
    }
}
