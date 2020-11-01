using Microsoft.AspNetCore.Http;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.CustomIdentity.JwtManager.Abstract;
using Pertuk.Business.Extensions.EmailExt;
using Pertuk.Business.Externals.Managers.Abstract;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Contracts.V1.Requests.Auth;
using Pertuk.Contracts.V1.Responses.Auth;
using Pertuk.Entities.Models;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly PertukUserManager _pertukUserManager;
        private readonly IEmailSender _emailSender;
        private readonly IUploadImageService _uploadImageService;
        private readonly IJwtManager _jwtManager;
        private readonly IFacebookAuthenticationManager _facebookAuthenticationManager;
        public AuthService(IEmailSender emailSender,
                            PertukUserManager pertukUserManager,
                            IUploadImageService uploadImageService,
                            IJwtManager jwtManager,
                            IFacebookAuthenticationManager facebookAuthenticationManager)
        {
            _jwtManager = jwtManager;
            _emailSender = emailSender;
            _pertukUserManager = pertukUserManager;
            _uploadImageService = uploadImageService;
            _facebookAuthenticationManager = facebookAuthenticationManager;
        }

        #region Register

        public async Task<AuthenticationResponseModel> RegisterAsync(RegisterRequestModel registerRequestModel)
        {
            if (registerRequestModel == null) throw new PertukApiException("Details should not be empty!");

            await CheckAndVerifyUserDetailForRegistering(registerRequestModel.Email);

            var profileImagePath = await UploadProfilePicture(registerRequestModel.ProfileImage, registerRequestModel.Email);

            var emailStartLocation = registerRequestModel.Email.IndexOf('@', StringComparison.Ordinal);

            var applicationIdentity = new ApplicationUser
            {
                UserName = registerRequestModel.Email.Substring(0, emailStartLocation),
                Email = registerRequestModel.Email,
                CreatedAt = DateTime.Now,
                ProfileImagePath = profileImagePath,
                RegisterFrom = "Pertuk"
            };

            var registerResult = await _pertukUserManager.CreateAsync(applicationIdentity, registerRequestModel.Password);

            if (registerResult.Succeeded)
            {
                await GenerateAndSendEmailConfirmationDigitCode(applicationIdentity);

                var token = await _jwtManager.GenerateToken(applicationIdentity);

                return new AuthenticationResponseModel
                {
                    Message = "User created successfully!",
                    Token = token.AccessToken
                };
            }

            throw new PertukApiException();
        }

        #endregion

        #region Facebook Auth

        public async Task<AuthenticationResponseModel> FacebookAuthenticationAsync(FacebookAuthRequestModel facebookAuthRequestModel)
        {
            if (string.IsNullOrEmpty(facebookAuthRequestModel.AccessToken)) throw new PertukApiException("Invalid Attempt");

            var validateToken = await _facebookAuthenticationManager.ValidateAccessTokenAsync(facebookAuthRequestModel.AccessToken);

            if (!validateToken.FacebookTokenValidationData.IsValid) throw new PertukApiException("Invalid Attempt");

            var userInfo = await _facebookAuthenticationManager.GetUserInfoAsync(facebookAuthRequestModel.AccessToken);

            var userDetail = await _pertukUserManager.FindByEmailAsync(userInfo.Email);

            // User not Exist Then Create new one!
            if (userDetail == null)
            {
                var emailStartLocation = userInfo.Email.IndexOf('@', StringComparison.Ordinal);

                var identity = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = $"{userInfo.Firstname} {userInfo.Lastname}",
                    Email = userInfo.Email,
                    UserName = userInfo.Email.Substring(0, emailStartLocation),
                    CreatedAt = DateTime.Now,
                    ProfileImagePath = userInfo.FacebookPicture.FacebookPictureData.Url.OriginalString,
                    RegisterFrom = "Facebook"
                };


                var createResult = await _pertukUserManager.CreateAsync(identity);

                if (createResult.Succeeded)
                {
                    var token = await _jwtManager.GenerateToken(identity);
                    return new AuthenticationResponseModel
                    {
                        Message = "User Registered With Facebook!",
                        Token = token.AccessToken
                    };
                }

                throw new PertukApiException();
            }

            // User Exist
            var loginToken = await _jwtManager.GenerateToken(userDetail);

            return new AuthenticationResponseModel
            {
                Message = "User Logged In With Facebook!",
                Token = loginToken.AccessToken
            };
        }

        #endregion

        #region Login
        public async Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel)
        {
            if (loginRequestModel == null) throw new PertukApiException("Please provide required information!");

            var getUserDetail = await _pertukUserManager.GetUserDetailByEmailAsync(loginRequestModel.Email);
            if (getUserDetail == null) throw new PertukApiException("User not found!");

            _pertukUserManager.CheckUserBanAndDeletion(getUserDetail.Id);

            var isUserHasPassword = await _pertukUserManager.HasPasswordAsync(getUserDetail);
            if (isUserHasPassword == false) // User registered from OAuth...
            {
                switch (getUserDetail.RegisterFrom) // Check Registered OAuth Service..
                {
                    case "Facebook":
                        throw new PertukApiException("User has registered with Facebook, Please try countinue with your Facebook account!");
                    default:
                        throw new PertukApiException();
                }
            }

            var checkUserPassword = await _pertukUserManager.CheckPasswordAsync(getUserDetail, loginRequestModel.Password);
            if (!checkUserPassword) throw new PertukApiException("Password is invalid!");

            var token = await _jwtManager.GenerateToken(getUserDetail);

            return new AuthenticationResponseModel
            {
                Message = "User Logged In",
                Token = token.AccessToken,
                RefreshToken = token.RefreshToken
            };

            throw new PertukApiException();
        }

        #endregion

        #region Email Confirmation

        public async Task<AuthenticationResponseModel> ConfirmEmailAsync(ConfirmEmailRequestModel confirmEmailRequest)
        {
            if (confirmEmailRequest == null) throw new PertukApiException("Please provide required information!");

            if (string.IsNullOrEmpty(confirmEmailRequest.DigitCode) && string.IsNullOrWhiteSpace(confirmEmailRequest.DigitCode)) throw new PertukApiException("Invalid digit code!");

            var userDetail = await _pertukUserManager.FindByIdAsync(confirmEmailRequest.UserId);
            if (userDetail == null) throw new PertukApiException("User not found!");

            var isEmailConfirmed = await _pertukUserManager.IsEmailConfirmedAsync(userDetail);
            if (isEmailConfirmed) throw new PertukApiException("Email already confirmed!");

            var confirmEmailResult = await _pertukUserManager.ConfirmEmailWithDigitCodeAsync(userDetail, confirmEmailRequest.DigitCode);

            if (confirmEmailResult.Succeeded)
            {
                return new AuthenticationResponseModel
                {
                    Message = "Email successfully confirmed!"
                };
            }

            throw new PertukApiException();
        }

        public async Task<AuthenticationResponseModel> SendEmailConfirmationCodeAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId) && string.IsNullOrWhiteSpace(userId)) throw new PertukApiException("Please provide required information!");

            var userDetail = await _pertukUserManager.FindByIdAsync(userId);

            if (userDetail == null) throw new PertukApiException("User not found!");

            var isEmailConfirmed = await _pertukUserManager.IsEmailConfirmedAsync(userDetail);
            if (isEmailConfirmed)
            {
                throw new PertukApiException("Email Address Already Confirmed!");
            }

            await GenerateAndSendEmailConfirmationDigitCode(userDetail);

            return new AuthenticationResponseModel
            {
                Message = "Email Confirmation digit code has successfuly sent to your Mail address! Please check your inbox!"
            };
        }

        #endregion

        #region Reset Password

        public async Task<AuthenticationResponseModel> SendResetPasswordCodeAsync(ForgotPasswordRequestModel forgotPasswordRequest)
        {
            if (forgotPasswordRequest == null) throw new PertukApiException("Please provide required information!");

            var userDetail = await _pertukUserManager.FindByEmailAsync(forgotPasswordRequest.Email);

            if (userDetail == null) throw new PertukApiException("User not found!");

            await GenerateAndSendResetPasswordLink(userDetail);

            return new AuthenticationResponseModel
            {
                Message = "Reset Password digit code has successfuly sent to your Mail address! Please check your inbox!"
            };
        }

        public async Task<AuthenticationResponseModel> ResetPasswordAsync(ResetPasswordRequestModel resetPasswordRequest)
        {
            if (string.IsNullOrEmpty(resetPasswordRequest.Email) && string.IsNullOrWhiteSpace(resetPasswordRequest.Email) || string.IsNullOrEmpty(resetPasswordRequest.DigitCode) && string.IsNullOrWhiteSpace(resetPasswordRequest.DigitCode)) throw new PertukApiException("Please provide required information!");

            var userDetail = await _pertukUserManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (userDetail == null) throw new PertukApiException("User not found!");

            var resetPasswordResult = await _pertukUserManager.ResetPasswordWithDigitCodeAsync(userDetail, resetPasswordRequest.DigitCode, resetPasswordRequest.NewPassword);

            if (resetPasswordResult.Succeeded)
            {
                return new AuthenticationResponseModel
                {
                    Message = "Password has successfully changed"
                };
            }

            throw new PertukApiException();
        }

        #endregion

        #region RefreshToken

        public async Task<AuthenticationResponseModel> RefreshTokenAsync(RefreshTokenRequestModel refreshTokenRequest)
        {
            if (refreshTokenRequest == null) throw new PertukApiException("Please provide required information!");

            var refreshTokenResult = await _jwtManager.RefreshTokenAsync(refreshTokenRequest);

            return new AuthenticationResponseModel
            {
                Token = refreshTokenResult.AccessToken,
                RefreshToken = refreshTokenResult.RefreshToken,
                Message = "Token has been refreshed!"
            };
        }

        #endregion

        #region Private Functions

        private async Task GenerateAndSendEmailConfirmationDigitCode(ApplicationUser userDetail)
        {
            var confirmationDigitCode = await _pertukUserManager.GenerateDigitCodeForEmailConfirmationAsync(userDetail);

            try
            {
                await _emailSender.SendEmailConfirmation(confirmationDigitCode, userDetail.Email, userDetail.Fullname);
            }
            catch (Exception)
            {
                throw new PertukApiException();
            }
        }

        private async Task GenerateAndSendResetPasswordLink(ApplicationUser userDetail)
        {
            var resetPasswordDigitCode = await _pertukUserManager.GenerateRecoveryCodeForResetPasswordAsync(userDetail);

            try
            {
                await _emailSender.SendResetPassword(resetPasswordDigitCode, userDetail.Email, userDetail.Fullname);
            }
            catch (Exception)
            {
                throw new PertukApiException();
            }
        }

        private async Task CheckAndVerifyUserDetailForRegistering(string email, string username = null)
        {
            var isEmailExist = await _pertukUserManager.FindByEmailAsync(email);
            if (isEmailExist != null)
            {
                _pertukUserManager.CheckUserBanAndDeletion(isEmailExist.Id);
                throw new PertukApiException("Email address already exist!");
            }

            if (username != null)
            {
                var isUsernameExist = await _pertukUserManager.FindByNameAsync(username);
                if (isUsernameExist != null)
                {
                    _pertukUserManager.CheckUserBanAndDeletion(isUsernameExist.Id);
                    throw new PertukApiException("Username already exist!");
                }
            }
        }

        private async Task<string> UploadProfilePicture(IFormFile profileImage, string email)
        {
            var trimmedEmail = email.Trim();
            var imagePathUrl = await _uploadImageService.UploadProfilePicture(profileImage, trimmedEmail + "_pp");

            return imagePathUrl;
        }

        #endregion
    }
}