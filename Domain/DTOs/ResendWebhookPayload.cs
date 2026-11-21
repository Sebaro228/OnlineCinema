using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ResendWebhookPayload
    {
        public string Type { get; set; } = string.Empty;
        public ResendWebhookData? Data { get; set; }
    }
}
