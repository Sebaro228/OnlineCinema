using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class DeleteProfile
    {
        public string UserName { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
