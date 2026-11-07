using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ProviderAuthRequest
    {
        public string ProviderToken { get; set; }
        public Provider ProviderType { get; set; }
    }
}
