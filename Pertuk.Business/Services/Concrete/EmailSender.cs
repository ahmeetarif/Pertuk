using Microsoft.Extensions.Configuration;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class EmailSender : IEmailSender
    {
        #region Private Variables

        private readonly IConfiguration _configuration;
        public SendGridEmailSettings SendGridEmailSetting { get; set; }

        #endregion

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            SendGridEmailSetting = new SendGridEmailSettings();
            _configuration.GetSection(nameof(SendGridEmailSettings)).Bind(SendGridEmailSetting);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var apiKey = SendGridEmailSetting.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(SendGridEmailSetting.FromEmail, SendGridEmailSetting.FromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var response = await client.SendEmailAsync(msg);
        }
    }
}