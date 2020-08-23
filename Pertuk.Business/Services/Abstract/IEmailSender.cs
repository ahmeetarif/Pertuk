using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
