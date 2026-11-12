using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ParticipantDto
    {
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
