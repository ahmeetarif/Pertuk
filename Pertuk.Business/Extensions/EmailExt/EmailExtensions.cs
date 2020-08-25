using FluentValidation.Results;
using MimeKit;
using Pertuk.Business.Services.Abstract;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pertuk.Business.Extensions.EmailExt
{
    public static class EmailExtensions
    {
        public static async Task SendEmailConfirmation(this IEmailSender emailSender, string digitCode, string email, string fullname)
        {
            var messageSubject = "Pertuk Email Confirmation";
            var messageBody = string.Empty;

            using (var sourceReader = File.OpenText("wwwroot/Templates/Email_Templates/Email_Confirmation_Template.html"))
            {
                messageBody = sourceReader.ReadToEnd();
                sourceReader.Close();
            }

            messageBody = messageBody.Replace("[digitcode]", digitCode);
            messageBody = messageBody.Replace("[fullname]", fullname);

            await emailSender.SendEmailAsync(email, messageSubject, messageBody);
        }
    }
}