using Domain.Models;
using Google.Apis.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Resend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class EmailService(IResend resendClient, UserManager<User> userManager)
    {
        public async Task SendEmailAsync(string to, string subject, string body, string email, string name, string? filePath)
        {
            var message = new EmailMessage
            {
                From = $"{name} <{email}>",
                Subject = subject,
                HtmlBody = body,
                TextBody = body
            };
            if (System.IO.File.Exists(filePath))
            {
                message.Attachments.Add(new EmailAttachment
                {
                    Filename = filePath,
                    Content = await System.IO.File.ReadAllBytesAsync(filePath),
                });
            }
            message.To.Add(to);
            var response = await resendClient.EmailSendAsync(message);
        }
    }
}