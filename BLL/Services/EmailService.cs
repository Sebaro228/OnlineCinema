using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Resend;

namespace BLL.Services
{
    public class EmailService(IConfiguration configuration, IResend resendClient)
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var key = configuration["Resend:ApiKey"];
            var email = configuration["Resend:FromEmail"];
            var name = configuration["Resend:FromName"];
            var message = new EmailMessage
            {
                From = $"{name} <{email}>",
                Subject = subject,
                HtmlBody = body,
                TextBody = body
            };
            message.To.Add(to);
            var response = await resendClient.EmailSendAsync(message);
        }
    }
}