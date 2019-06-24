using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MacSlopes.Extensions;
using MacSlopes.Services.Abstract;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MacSlopes.Services.Implementation
{
    public class EmailSender : IEmailSender
    {
        //public AuthMessageSenderOptions _options;

        //public EmailSender(IOptions<AuthMessageSenderOptions> options)
        //{
        //     _options = options.Value;
        //}
        public Task SendMail(string email, string subject, string body)
        {
           return Execute("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", subject, body, email);
        }

        private Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("daemongriffons@gmail.com", "Daemon Griffons"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message,
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
