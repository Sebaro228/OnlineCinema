using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BLL.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinChat(string chatRoomName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomName);
            await Clients.OthersInGroup(chatRoomName).SendAsync("UserJoined", $"{userName} joined the chat.");
        }
        public async Task LeaveChat(string chatRoomName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomName);
            await Clients.Group(chatRoomName).SendAsync("UserLeft", $"{userName} left the chat.");
        }
        public async Task SendMessageToChat(string chatRoomName, string userName, string message)
        {
            await Clients.Group(chatRoomName).SendAsync("ReceiveMessage", userName, message);
        }
    }
}
