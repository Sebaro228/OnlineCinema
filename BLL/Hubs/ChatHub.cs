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
        // Вход пользователя в конкретный чат
        public async Task JoinChat(string chatRoomName, string userName)
        {
            // Добавляем текущее подключение в группу
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomName);

            // Уведомляем остальных участников этого чата
            await Clients.OthersInGroup(chatRoomName)
                .SendAsync("UserJoined", $"{userName} присоединился к чату.");
        }

        // Выход пользователя из чата
        public async Task LeaveChat(string chatRoomName, string userName)
        {
            // Удаляем подключение из группы
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomName);

            // Уведомляем участников чата
            await Clients.Group(chatRoomName)
                .SendAsync("UserLeft", $"{userName} покинул чат.");
        }

        // Отправка сообщения только в выбранный чат
        public async Task SendMessageToChat(string chatRoomName, string userName, string message)
        {
            // Рассылаем сообщение всем, кто находится в указанной группе
            await Clients.Group(chatRoomName)
                .SendAsync("ReceiveMessage", userName, message);
        }
    }
}
