using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class EmailService(IConfiguration configuration)
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var key = configuration["SendGrid:ApiKey"];
            var email = configuration["SendGrid:FromEmail"];
            var name = configuration["SendGrid:FromName"];
            var client = new SendGridClient(key);
            var from = new EmailAddress(email, name);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);
            var responce = await client.SendEmailAsync(msg);
        }
    }
}