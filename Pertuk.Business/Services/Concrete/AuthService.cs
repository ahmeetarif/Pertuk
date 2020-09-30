using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.Extensions.EmailExt;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Common.Infrastructure;
using Pertuk.Dto.Requests.Auth;
using Pertuk.Dto.Responses.Auth;
using Pertuk.Entities.Models;
using System;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class AuthService : IAuthService
    {
        #region Private Variables

        private readonly PertukUserManager _pertukUserManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IUploadImageService _uploadImageService;

        #endregion

        public AuthService(ITokenService tokenService,
                            IEmailSender emailSender,
                            PertukUserManager pertukUserManager,
                            IFacebookAuthService facebookAuthService,
                            IUploadImageService uploadImageService)
        {
            _tokenService = tokenService;
            _emailSender = emailSender;
            _pertukUserManager = pertukUserManager;
            _facebookAuthService = facebookAuthService;
            _uploadImageService = uploadImageService;
        }

        #region Register Student and Teacher Methods


        //public async Task<AuthenticationResponseModel> RegisterStudentAsync(StudentUserRegisterRequestModel studentUser)
        //{
        //    if (studentUser == null) throw new PertukApiException(BaseErrorResponseMessages.User.EnterUserDetail);

        //    await CheckAndVerifyUserDetailForRegistering(studentUser.Email);

        //    var profileImagePath = await UploadProfilePicture(studentUser.ProfileImage, studentUser.Email);

        //    var applicationIdentity = new ApplicationUser
        //    {
        //        Fullname = studentUser.Fullname,
        //        UserName = studentUser.Username ?? studentUser.Email,
        //        Email = studentUser.Email,
        //        CreatedAt = DateTime.Now,
        //        ProfileImagePath = profileImagePath
        //    };

        //    var passwordHasher = await _pertukUserManager.UpdatePasswordHash(applicationIdentity, studentUser.Password);
        //    if (!passwordHasher.Succeeded) throw new PertukApiException();

        //    var studentIdentity = new StudentUsers
        //    {
        //        UserId = applicationIdentity.Id,
        //        Grade = studentUser.Grade,
        //        User = applicationIdentity,
        //        Department = studentUser.Department
        //    };

        //    var result = await _pertukUserManager.CreateStudent(studentIdentity);

        //    if (result == EntityState.Added)
        //    {
        //        await GenerateAndSendEmailConfirmationLink(applicationIdentity);

        //        var token = _tokenService.GenerateToken(applicationIdentity);

        //        return new AuthenticationResponseModel
        //        {
        //            Message = "User created successfully!",
        //            Token = token
        //        };
        //    }

        //    throw new PertukApiException();
        //}

        //public async Task<AuthenticationResponseModel> RegisterTeacherAsync(TeacherUserRegisterRequestModel teacherUser)
        //{
        //    if (teacherUser == null) throw new PertukApiException(BaseErrorResponseMessages.User.EnterUserDetail);

        //    await CheckAndVerifyUserDetailForRegistering(teacherUser.Email, teacherUser.Username);

        //    var profileImagePath = await UploadProfilePicture(teacherUser.ProfileImage, teacherUser.Email);

        //    var applicationIdentity = new ApplicationUser
        //    {
        //        Fullname = teacherUser.Fullname,
        //        UserName = teacherUser.Username ?? teacherUser.Email,
        //        Email = teacherUser.Email,
        //        CreatedAt = DateTime.Now,
        //        ProfileImagePath = profileImagePath
        //    };

        //    var passwordHasher = await _pertukUserManager.UpdatePasswordHash(applicationIdentity, teacherUser.Password);
        //    if (!passwordHasher.Succeeded) throw new PertukApiException();

        //    var teacherIdentity = new TeacherUsers
        //    {
        //        UserId = applicationIdentity.Id,
        //        User = applicationIdentity,
        //        ForStudent = teacherUser.ForStudent,
        //        UniversityName = teacherUser.UniversityName,
        //        DepartmentOf = teacherUser.DepartmentOf,
        //        AcademicOf = teacherUser.AcademicOf,
        //        Subject = teacherUser.Subject,
        //        YearsOfExperience = teacherUser.YearsOfExperience
        //    };

        //    var result = await _pertukUserManager.CreateTeacher(teacherIdentity);

        //    if (result == EntityState.Added)
        //    {
        //        await GenerateAndSendEmailConfirmationLink(applicationIdentity);

        //        var token = _tokenService.GenerateToken(applicationIdentity);

        //        return new AuthenticationResponseModel
        //        {
        //            Message = "User created successfully",
        //            Token = token
        //        };
        //    }

        //    throw new PertukApiException();
        //}

        #endregion

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
                ProfileImagePath = profileImagePath
            };

            var registerResult = await _pertukUserManager.CreateAsync(applicationIdentity, registerRequestModel.Password);

            if (registerResult.Succeeded)
            {
                await GenerateAndSendEmailConfirmationLink(applicationIdentity);

                var token = _tokenService.GenerateToken(applicationIdentity);

                return new AuthenticationResponseModel
                {
                    Message = "User created successfully!",
                    Token = token
                };
            }

            throw new PertukApiException();
        }

        #endregion

        #region Facebook Auth

        public async Task<AuthenticationResponseModel> FacebookAuthentication(FacebookAuthRequestModel facebookAuthRequestModel)
        {
            if (string.IsNullOrEmpty(facebookAuthRequestModel.AccessToken)) throw new PertukApiException("Invalid Attempt");

            var validateToken = await _facebookAuthService.ValidateAccessTokenAsync(facebookAuthRequestModel.AccessToken);

            if (!validateToken.FacebookTokenValidationData.IsValid) throw new PertukApiException("Invalid Attempt");

            var userInfo = await _facebookAuthService.GetUserInfoAsync(facebookAuthRequestModel.AccessToken);

            var userDetail = await _pertukUserManager.GetUserDetailByEmailAsync(userInfo.Email);

            // User not Exist Then Create new one!
            if (userDetail == null)
            {
                var identity = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = $"{userInfo.Firstname} {userInfo.Lastname}",
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                    CreatedAt = DateTime.Now,
                    ProfileImagePath = userInfo.FacebookPicture.FacebookPictureData.Url.OriginalString
                };


                var createResult = await _pertukUserManager.CreateAsync(identity);

                if (createResult.Succeeded)
                {
                    var token = _tokenService.GenerateToken(identity, userInfo.FacebookPicture.FacebookPictureData);
                    return new AuthenticationResponseModel
                    {
                        Message = "User Registered With Facebook!",
                        Token = token
                    };
                }

                throw new PertukApiException();
            }

            // User Exist
            var loginToken = _tokenService.GenerateToken(userDetail, userInfo.FacebookPicture.FacebookPictureData);

            return new AuthenticationResponseModel
            {
                Message = "User Logged In With Facebook!",
                Token = loginToken
            };
        }

        #endregion

        public async Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel)
        {
            if (loginRequestModel == null) throw new PertukApiException("Please provide required information!");

            var getUserDetail = await _pertukUserManager.GetUserDetailByNameAsync(loginRequestModel.Username);
            if (getUserDetail == null) throw new PertukApiException("User not found!");

            _pertukUserManager.CheckUserBanAndDeletion(getUserDetail.Id);

            var checkUserPassword = await _pertukUserManager.CheckPasswordAsync(getUserDetail, loginRequestModel.Password);
            if (!checkUserPassword) throw new PertukApiException("Password is invalid!");

            var token = _tokenService.GenerateToken(getUserDetail);
            return new AuthenticationResponseModel
            {
                Message = "User Logged In",
                Token = token
            };

            throw new PertukApiException();
        }

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

        public async Task<AuthenticationResponseModel> SendEmailConfirmation(string userId)
        {
            if (string.IsNullOrEmpty(userId) && string.IsNullOrWhiteSpace(userId)) throw new PertukApiException("Please provide required information!");

            var userDetail = await _pertukUserManager.FindByIdAsync(userId);

            if (userDetail == null) throw new PertukApiException("User not found!");

            var isEmailConfirmed = await _pertukUserManager.IsEmailConfirmedAsync(userDetail);

            if (isEmailConfirmed)
            {
                throw new PertukApiException("Email Address Already Confirmed!");
            }

            await GenerateAndSendEmailConfirmationLink(userDetail);

            return new AuthenticationResponseModel
            {
                Message = "Email Confirmation digit code has successfuly sent to your Mail address! Please check your inbox!"
            };
        }

        public async Task<AuthenticationResponseModel> SendResetPasswordLink(ForgotPasswordRequestModel forgotPasswordRequest)
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

        public async Task<AuthenticationResponseModel> ResetPassword(ResetPasswordRequestModel resetPasswordRequest)
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

        #region Private Functions

        private async Task GenerateAndSendEmailConfirmationLink(ApplicationUser userDetail)
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
            var resetPasswordDigitCode = await _pertukUserManager.GenerateDigitCodeForEmailConfirmationAsync(userDetail);

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