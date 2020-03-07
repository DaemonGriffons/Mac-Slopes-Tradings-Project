using System.Threading.Tasks;

namespace MacSlopes.Services.Abstract
{
    public interface IEmailSender
    {
        Task SendMail(string email, string subject, string body);

        Task ContactEmail(string email, string Name, string subject, string body);
    }
}
