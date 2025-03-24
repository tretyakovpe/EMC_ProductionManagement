using Microsoft.AspNetCore.SignalR;

namespace ProductionManagement.Hubs
{
    public class LogHub : Hub
    {
        public async Task SendLog(string message)
        {
            await Clients.All.SendAsync("ReceiveLog", message);
        }
    }
}