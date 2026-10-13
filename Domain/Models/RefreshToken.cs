using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expired { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public bool isRevoked { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public bool IsExpired => DateTime.Now >= Expired;
        public bool IsActive => !isRevoked && !IsExpired;
    }
}
