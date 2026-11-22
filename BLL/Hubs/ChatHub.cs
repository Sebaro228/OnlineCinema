using Domain.DTOs;
using Domain.Models;
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
        public async Task SendMessageToChat(string chatRoomName, string userName, MessageDto message)
        {
            await Clients.Group(chatRoomName).SendAsync("ReceiveMessage", userName, message.Content);
            if (File.Exists(message.FilePath))
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(message.FilePath);
                await Clients.Group(chatRoomName).SendAsync("ReceiveFile", userName, bytes);
            }
        }
    }
}
