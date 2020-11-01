using Pertuk.EmailService.Abstract;
using Pertuk.EmailService.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Pertuk.EmailService.Concrete
{
    public class EmailSender : IEmailSender
    {
        private readonly SendGridEmailOptions _sendGridEmailOptions;

        public EmailSender(SendGridEmailOptions sendGridEmailOptions)
        {
            _sendGridEmailOptions = sendGridEmailOptions;
        }

        public async Task SendEmailAsync(string clientEmail, string subject, string message)
        {
            var messageContent = CreateMessage(subject, message, clientEmail);
            await Send(messageContent);

        }

        #region Private Functions

        private SendGridMessage CreateMessage(string subject, string message, string clientEmail)
        {
            var fromEmailAddress = new EmailAddress(_sendGridEmailOptions.FromEmail);
            var clientEmailAddress = new EmailAddress(clientEmail);
            var sendGridMessage = MailHelper.CreateSingleEmail(fromEmailAddress, clientEmailAddress, subject, message, message);
            return sendGridMessage;
        }

        private async Task Send(SendGridMessage message)
        {
            var client = new SendGridClient(_sendGridEmailOptions.ApiKey);
            try
            {
                await client.SendEmailAsync(message);
            }
            catch (Exception)
            {
                //TODO: Log Exception
            }
        }

        #endregion

    }
}