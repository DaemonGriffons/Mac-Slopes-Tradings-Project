using MacSlopes.Services.Abstract;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

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

        public Task ContactEmail(string email, string Name, string subject, string body)
        {
            return Mail("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", email, subject, Name, body);
        }
        private Task Mail(string apiKey, string Email, string Subject,string Name,string Body)
        {
            var client = new SendGridClient(apiKey);
            var message = new SendGridMessage()
            {
                From = new EmailAddress(Email, Name),
                Subject = Subject,
                PlainTextContent = Body,
                HtmlContent = Body
            };

            message.AddTo("daemongriffons@gmail.com", "Daemon Griffons");
            message.SetClickTracking(false, false);

            return client.SendEmailAsync(message);
        }
    }
}
