using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
namespace POYA.Unities.Services
{
    public class EmailSender : IEmailSender //  Controllers
    {
        public IConfiguration _configuration { get; }
        public EmailSender(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var userName = _configuration["EmailSender:userName"]; //(string)jsonObject[nameof(EmailSender)]["userName"];
            var host = _configuration["EmailSender:host"];// (string)jsonObject[nameof(EmailSender)]["host"];
            var password = _configuration["EmailSender:password"];//     (string)jsonObject[nameof(EmailSender)]["password"];
            var port = Convert.ToInt32(_configuration["EmailSender:port"]);//    (short)jsonObject[nameof(EmailSender)]["port"];
            var enableSsl = Convert.ToBoolean(_configuration["EmailSender:enableSsl"]);
            using (var smtpClient = new SmtpClient(host: host, port: port)
            {
                EnableSsl = enableSsl,
                Credentials = new System.Net.NetworkCredential(userName: userName, password: password),
            })
            {
                var mailMessage = new MailMessage(subject: subject, body: htmlMessage, from: userName, to: email)
                {
                    IsBodyHtml = true,
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8
                };
                return smtpClient.SendMailAsync(mailMessage);
            }
            //throw new NotImplementedException();
        }
    }
}
