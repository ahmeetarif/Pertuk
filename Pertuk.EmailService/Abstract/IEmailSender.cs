using System.Threading.Tasks;

namespace Pertuk.EmailService.Abstract
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string clientEmail, string subject, string message);
    }
}