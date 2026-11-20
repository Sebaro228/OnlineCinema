using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class MessageDto
    {
        public string Content { get; set; } = string.Empty;
        public string? FilePath { get; set; }
    }
}
