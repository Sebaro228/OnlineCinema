using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email for password resetting")]
        public string Email { get; set; }
    }
}
