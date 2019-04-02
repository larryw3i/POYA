using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IHostingEnvironment _hostingEnv;

        public EmailSender(IHostingEnvironment hostingEnv)
        {
            _hostingEnv = hostingEnv;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var file = File.OpenText(_hostingEnv.ContentRootPath+ "/appsettings.json");
            var reader = new JsonTextReader(file);
            var jsonObject = (JObject)JToken.ReadFrom(reader);
            file.Close(); 

            var userName =(string)jsonObject[nameof(EmailSender)]["userName"];
            var host = (string)jsonObject[nameof(EmailSender)]["host"];
            var password = (string)jsonObject[nameof(EmailSender)]["password"];

            var port =(short)jsonObject[nameof(EmailSender)]["port"];

            var smtpClient = new SmtpClient(host: host, port: port)
            {
                EnableSsl = false,
                Credentials = new System.Net.NetworkCredential(userName: userName, password: password),
            };
            var mailMessage = new MailMessage(subject: subject, body: htmlMessage, from: userName, to: email)
            {
                IsBodyHtml = true,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };
            return smtpClient.SendMailAsync(mailMessage);
            //throw new NotImplementedException();
        }

    }
}
