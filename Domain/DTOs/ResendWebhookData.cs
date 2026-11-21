using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ResendWebhookData
    {
        public Guid EmailId { get; set; } 
        public string From { get; set; } = string.Empty;
        public List<string> To { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
    }
}
