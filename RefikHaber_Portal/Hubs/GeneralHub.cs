using Microsoft.AspNetCore.SignalR;

namespace RefikHaber.Hubs;

public class GeneralHub : Hub
{
    public async Task NotifyAdmins(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);    
    }
}