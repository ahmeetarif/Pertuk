using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
