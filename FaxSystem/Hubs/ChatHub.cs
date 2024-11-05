using Microsoft.AspNetCore.SignalR;
using FaxSystem.Models;
namespace FaxSystem.Hubs
{
    public class ChatHub: Hub
    {
        public async Task SendMessage(string user, string message,int id,int type)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message,id,type);
        }
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}
    }
}
