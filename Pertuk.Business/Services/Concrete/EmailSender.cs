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

        #endregion

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var sendGridEmailSettings = new SendGridEmailSettings();
            _configuration.GetSection(nameof(SendGridEmailSettings)).Bind(sendGridEmailSettings);

            var apiKey = sendGridEmailSettings.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(sendGridEmailSettings.FromEmail, sendGridEmailSettings.FromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var response = await client.SendEmailAsync(msg);
        }
    }
}