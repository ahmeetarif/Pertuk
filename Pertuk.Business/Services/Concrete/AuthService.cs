using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Dto.Requests.Auth;
using Pertuk.Dto.Responses.Auth;
using Pertuk.Entities.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class AuthService : IAuthService
    {
        #region Private Variables

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        #endregion

        public AuthService(UserManager<ApplicationUser> userManager,
                            ITokenService tokenService,
                            IConfiguration configuration,
                            IEmailSender emailSender)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _emailSender = emailSender;
        }


        public async Task<AuthenticationResponseModel> RegisterAsync(RegisterRequestModel registerRequestModel)
        {
            // TODO : (optional)Throw Exception if MODEL is NULL
            if (registerRequestModel == null)
                return new AuthenticationResponseModel
                {
                    IsSuccess = false,
                    Errors = new[] { "Please enter new user detail!" }
                };

            var isEmailExist = await _userManager.FindByEmailAsync(registerRequestModel.Email) != null;
            if (isEmailExist)
                return new AuthenticationResponseModel
                {
                    IsSuccess = false,
                    Errors = new[] { "Email address already exist!" }
                };

            var isUsernameExist = await _userManager.FindByNameAsync(registerRequestModel.Username) != null;
            if (isUsernameExist)
                return new AuthenticationResponseModel
                {
                    IsSuccess = false,
                    Errors = new[] { "Username already exist!" }
                };

            var newUserDetail = new ApplicationUser
            {
                Firstname = registerRequestModel.Firstname,
                Lastname = registerRequestModel.Lastname,
                UserName = registerRequestModel.Username,
                Email = registerRequestModel.Email,
                CreatedAt = DateTime.Now
            };

            var createResult = await _userManager.CreateAsync(newUserDetail, registerRequestModel.Password);

            if (createResult.Succeeded)
            {
                // TODO : Log Register to Elasticsearch
                var token = _tokenService.CreateToken(newUserDetail);

                return new AuthenticationResponseModel
                {
                    IsSuccess = true,
                    Token = token
                };
            }

            // TODO : (optional) - Log errors to Elasticsearch
            return new AuthenticationResponseModel
            {
                IsSuccess = false,
                Errors = createResult.Errors.Select(x => x.Description)
            };
        }

        public async Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel)
        {
            if (loginRequestModel == null)
                return new AuthenticationResponseModel
                {
                    IsSuccess = false,
                    Errors = new[] { "Please enter user detail!" }
                };

            var getUserDetail = await _userManager.FindByNameAsync(loginRequestModel.Username);

            if (getUserDetail == null)
                return new AuthenticationResponseModel
                {
                    IsSuccess = false,
                    Errors = new[] { string.Format("User with Username : {0} not exist!", loginRequestModel.Username) }
                };

            var checkUserPassword = await _userManager.CheckPasswordAsync(getUserDetail, loginRequestModel.Password);

            if (!checkUserPassword)
                return new AuthenticationResponseModel
                {
                    IsSuccess = false,
                    Errors = new[] { string.Format("Password is incorrect! for User : {0} ", loginRequestModel.Username) }
                };

            var token = _tokenService.CreateToken(getUserDetail);

            GenerateAndSendEmailConfirmationLink(getUserDetail);

            return new AuthenticationResponseModel
            {
                IsSuccess = true,
                Token = token
            };
        }

        public async Task<UserResponseModel> ConfirmEmailAsync(ConfirmEmailRequestModel confirmEmailRequest)
        {
            if (string.IsNullOrEmpty(confirmEmailRequest.Token) && string.IsNullOrWhiteSpace(confirmEmailRequest.Token))
            {
                return new UserResponseModel
                {
                    IsSuccess = false,
                    Message = "Token is invalid!"
                };
            }

            var userDetail = await _userManager.FindByIdAsync(confirmEmailRequest.UserId);

            if (userDetail == null)
                return new UserResponseModel
                {
                    IsSuccess = false,
                    Message = "User not found!"
                };

            if (userDetail.EmailConfirmed)
            {
                return new UserResponseModel
                {
                    IsSuccess = true,
                    Message = "Email address already confirmed!"
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(confirmEmailRequest.Token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);

            var confirmationResult = await _userManager.ConfirmEmailAsync(userDetail, normalToken);

            if (confirmationResult.Succeeded)
            {
                return new UserResponseModel
                {
                    IsSuccess = true,
                    Message = "Email confirmed successfuly!"
                };
            }
            else
            {
                return new UserResponseModel
                {
                    IsSuccess = false,
                    Errors = confirmationResult.Errors.Select(x => x.Description),
                    Message = "Email did not confirmed!"
                };
            }
        }

        public async Task<UserResponseModel> SendEmailConfirmation(string userId)
        {
            var userDetail = await _userManager.FindByIdAsync(userId);

            if (userDetail == null)
                return new UserResponseModel
                {
                    IsSuccess = false,
                    Message = "User not found!"
                };

            GenerateAndSendEmailConfirmationLink(userDetail);

            return new UserResponseModel
            {
                IsSuccess = true,
                Message = "Confirmation link has successfuly sent to your email address!"
            };
        }

        public async Task<UserResponseModel> SendResetPasswordLink(ForgotPasswordRequestModel forgotPasswordRequest)
        {
            if (string.IsNullOrEmpty(forgotPasswordRequest.Email) && string.IsNullOrWhiteSpace(forgotPasswordRequest.Email))
                return new UserResponseModel
                {
                    IsSuccess = false,
                    Message = "Enter Email Address!"
                };

            var userDetail = await _userManager.FindByEmailAsync(forgotPasswordRequest.Email);

            if (userDetail == null)
                return new UserResponseModel
                {
                    IsSuccess = false,
                    Message = $"User with : {forgotPasswordRequest.Email} not found!"
                };

            GenerateAndSendResetPasswordLink(userDetail);

            return new UserResponseModel
            {
                IsSuccess = true,
                Message = "Reset password link has successfuly sent to your email address!"
            };
        }

        public async Task<UserResponseModel> ResetPassword(ResetPasswordRequestModel resetPasswordRequest)
        {
            if (string.IsNullOrEmpty(resetPasswordRequest.Email) && string.IsNullOrWhiteSpace(resetPasswordRequest.Email) || string.IsNullOrEmpty(resetPasswordRequest.Token) && string.IsNullOrWhiteSpace(resetPasswordRequest.Token))
                return new UserResponseModel
                {
                    IsSuccess = false,
                    Message = "Invalid URL!"
                };

            var userDetail = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (userDetail == null)
                return new UserResponseModel
                {
                    IsSuccess = false,
                    Message = $"User with : {resetPasswordRequest.Email} not found!"
                };

            var tokenFromWebDecoded = WebEncoders.Base64UrlDecode(resetPasswordRequest.Token);
            var validToken = Encoding.UTF8.GetString(tokenFromWebDecoded);

            var resetPasswordResult = await _userManager.ResetPasswordAsync(userDetail, validToken, resetPasswordRequest.NewPassword);

            if (resetPasswordResult.Succeeded)
            {
                return new UserResponseModel
                {
                    IsSuccess = true,
                    Message = "Reset password successful!"
                };
            }
            else
            {
                return new UserResponseModel
                {
                    IsSuccess = false,
                    Message = "Error while reseting your password!",
                    Errors = resetPasswordResult.Errors.Select(x => x.Description)
                };
            }
        }

        #region Private Functions

        private async void GenerateAndSendEmailConfirmationLink(ApplicationUser userDetail)
        {
            var mailOptions = new MailOptions();
            _configuration.GetSection(nameof(MailOptions)).Bind(mailOptions);

            var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(userDetail);
            var encodedConfirmationCode = Encoding.UTF8.GetBytes(confirmationCode);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedConfirmationCode);

            var url = $"{mailOptions.BaseUrl}/auth/confirmemail?userid={userDetail.Id}&token={validEmailToken}";

            await _emailSender.SendEmailAsync(userDetail.Email, "Pertuk Email Confirmation", url);
        }

        private async void GenerateAndSendResetPasswordLink(ApplicationUser userDetail)
        {
            var mailOptions = new MailOptions();
            _configuration.GetSection(nameof(MailOptions)).Bind(mailOptions);

            var resetPasswordCode = await _userManager.GeneratePasswordResetTokenAsync(userDetail);
            var resetPasswordCodeInBytes = Encoding.UTF8.GetBytes(resetPasswordCode);
            var encodedResetPasswordCode = WebEncoders.Base64UrlEncode(resetPasswordCodeInBytes);

            var url = $"{mailOptions.BaseUrl}/auth/resetpassword?email={userDetail.Email}&token={encodedResetPasswordCode}";

            await _emailSender.SendEmailAsync(userDetail.Email, "Pertuk Reset Password", url);
        }

        #endregion
    }
}