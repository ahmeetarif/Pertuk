using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pertuk.Business.Extensions.StringExt;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Pertuk.Business.Extensions.EmailExt
{
    public static class EmailExtensions
    {
        private static MediaOptions MediaOption { get; set; }
        private static readonly IConfigurationRoot _configuration;

        static EmailExtensions()
        {
            MediaOption = new MediaOptions();
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            _configuration.GetSection(nameof(MediaOptions)).Bind(MediaOption);
        }

        public static async Task SendEmailConfirmation(this IEmailSender emailSender, string digitCode, string email, string fullname)
        {
            string messageSubject = "Pertuk Email Confirmation";

            string messageBody = ConfigureEmailConfirmationMessageBody(digitCode, fullname);

            await emailSender.SendEmailAsync(email, messageSubject, messageBody);
        }

        #region Private Functions

        private static string ConfigureEmailConfirmationMessageBody(string digitCode, string fullname)
        {
            string messageBody = string.Empty;

            using (StreamReader sourceReader = File.OpenText("wwwroot/Templates/Email_Templates/Email_Confirmation_Template.html"))
            {
                messageBody = sourceReader.ReadToEnd();
                sourceReader.Close();
            }

            string templatePath = MediaOption.SitePath + MediaOption.TemplateDirectoryPath;

            Dictionary<string, string> replacements = new Dictionary<string, string>() { { "[digitcode]", digitCode }, { "[fullname]", fullname }, { "[pertuklogo]", templatePath + BaseMediaPaths.Templates.pertukLogo }, { "[whitedown]", templatePath + BaseMediaPaths.Templates.whiteDown }, { "[step1]", templatePath + BaseMediaPaths.Templates.step1 }, { "[step2]", templatePath + BaseMediaPaths.Templates.step2 }, { "[step3]", templatePath + BaseMediaPaths.Templates.step3 }, { "[facebook]", templatePath + BaseMediaPaths.Templates.facebook }, { "[twitter]", templatePath + BaseMediaPaths.Templates.twitter }, { "[instagram]", templatePath + BaseMediaPaths.Templates.instagram }, { "[welcomeemail]", templatePath + BaseMediaPaths.Templates.welcomeEmail } };

            messageBody = messageBody.ReplaceRange(replacements);

            return messageBody;
        }

        #endregion

    }
}