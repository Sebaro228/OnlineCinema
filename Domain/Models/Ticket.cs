using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Ticket
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string AgentId { get; set; } = string.Empty;
        public string Topic { get; set; } = "Without topic";
        public List<Message> Messages { get; set; } = new();
        public Conversation ConversationType { get; set; } 
        public bool IsOpen { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }

    }
}
