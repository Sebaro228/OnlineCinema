using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CreateTicket 
    {
        public string FirstMessage { get; set; } = string.Empty; 
        public string Topic { get; set; } = string.Empty;
        public string? FilePath { get; set; }
    }
}
