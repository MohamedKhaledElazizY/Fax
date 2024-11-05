using Microsoft.AspNetCore.SignalR;
using FaxSystem.Models;
namespace FaxSystem.Hubs
{
    public class CHub : Hub
    {
        public async Task SendMessage( int id, int type)
        {
            await Clients.All.SendAsync("ReceiveMessage", id, type);
        }
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}
    }
}
