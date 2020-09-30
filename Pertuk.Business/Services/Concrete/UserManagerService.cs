using Pertuk.Business.CustomIdentity;
using Pertuk.Business.Extensions.EmailExt;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Dto.Requests.UserManager;
using Pertuk.Dto.Responses.UserManager;
using System;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class UserManagerService : IUserManagerService
    {
        private readonly PertukUserManager _pertukUserManager;
        private readonly IEmailSender _emailSender;
        public UserManagerService(
            IEmailSender emailSender,
            PertukUserManager pertukUserManager)
        {
            _emailSender = emailSender;
            _pertukUserManager = pertukUserManager;
        }

        public async Task SetPoints(int points)
        {

        }

        public async Task UpdateUserDetail(ChangeUserDetailRequestModel changeUserDetailRequest)
        {
            if (changeUserDetailRequest == null) throw new PertukApiException("Please enter user detail!");



        }

        public async Task<UserManagerResponseModel> ChangeEmailAddress(ChangeEmailRequestModel changeEmailRequest)
        {
            if (changeEmailRequest == null) throw new PertukApiException("Please Enter Email Details!");

            var isUserValid = await _pertukUserManager.FindByEmailAsync(changeEmailRequest.OldEmail);
            if (isUserValid == null) throw new PertukApiException("User not found with this Email address!");

            var isNewEmailExist = await _pertukUserManager.FindByEmailAsync(changeEmailRequest.NewEmail);
            if (isNewEmailExist != null) throw new PertukApiException("User already exist with this new Email address!");

            var updateEmail = await _pertukUserManager.SetEmailAsync(isUserValid, changeEmailRequest.NewEmail);
            if (!updateEmail.Succeeded) throw new PertukApiException();

            var digitCodeForConfirmation = await _pertukUserManager.GenerateDigitCodeForEmailConfirmationAsync(isUserValid);

            try
            {
                await _emailSender.SendEmailConfirmation(digitCodeForConfirmation, changeEmailRequest.NewEmail, isUserValid.Fullname ?? "Pertuk Users");
            }
            catch (Exception) { throw new PertukApiException(); }

            return new UserManagerResponseModel
            {
                Message = "Email address has been changed! Please check your new Email address for confirmation!"
            };
        }
    }
}