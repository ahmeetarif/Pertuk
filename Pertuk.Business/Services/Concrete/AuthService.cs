using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.Extensions.EmailExt;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Common.Infrastructure;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Dto.Requests.Auth;
using Pertuk.Dto.Responses.Auth;
using Pertuk.Entities.Models;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class AuthService : IAuthService
    {
        #region Private Variables

        private readonly PertukUserManager _pertukUserManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly MediaOptions _mediaOptions;
        private readonly IFacebookAuthService _facebookAuthService;

        #endregion

        public AuthService(ITokenService tokenService,
                            IEmailSender emailSender,
                            PertukUserManager pertukUserManager,
                            IOptions<MediaOptions> mediaOptions,
                            IFacebookAuthService facebookAuthService)
        {
            _tokenService = tokenService;
            _emailSender = emailSender;
            _pertukUserManager = pertukUserManager;
            _mediaOptions = mediaOptions.Value;
            _facebookAuthService = facebookAuthService;
        }

        #region Register
        public async Task<AuthenticationResponseModel> RegisterStudentAsync(StudentUserRegisterRequestModel studentUser)
        {
            if (studentUser == null) throw new PertukApiException(BaseErrorResponseMessages.User.EnterUserDetail);

            await CheckAndVerifyUserDetailForRegistering(studentUser.Email);

            var defaultProfileImagePath = _mediaOptions.EmptyProfilePicture;

            var applicationIdentity = new ApplicationUser
            {
                Fullname = studentUser.Fullname,
                UserName = studentUser.Username ?? studentUser.Email,
                Email = studentUser.Email,
                CreatedAt = DateTime.Now,
                ProfileImagePath = defaultProfileImagePath
            };

            var passwordHasher = await _pertukUserManager.UpdatePasswordHash(applicationIdentity, studentUser.Password);
            if (!passwordHasher.Succeeded) throw new PertukApiException();

            var studentIdentity = new StudentUsers
            {
                UserId = applicationIdentity.Id,
                Grade = studentUser.Grade,
                User = applicationIdentity,
                Department = studentUser.Department
            };

            var result = await _pertukUserManager.CreateStudent(studentIdentity);

            if (result == EntityState.Added)
            {
                try { await GenerateAndSendEmailConfirmationLink(applicationIdentity); } catch (Exception) { throw new PertukApiException(); };

                var token = _tokenService.GenerateToken(applicationIdentity);

                return new AuthenticationResponseModel
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                    Token = token
                };
            }

            throw new PertukApiException();
        }

        public async Task<AuthenticationResponseModel> RegisterTeacherAsync(TeacherUserRegisterRequestModel teacherUser)
        {
            if (teacherUser == null) throw new PertukApiException(BaseErrorResponseMessages.User.EnterUserDetail);

            await CheckAndVerifyUserDetailForRegistering(teacherUser.Email, teacherUser.Username);

            var defaultProfileImagePath = _mediaOptions.EmptyProfilePicture;

            var applicationIdentity = new ApplicationUser
            {
                Fullname = teacherUser.Fullname,
                UserName = teacherUser.Username ?? teacherUser.Email,
                Email = teacherUser.Email,
                CreatedAt = DateTime.Now,
                ProfileImagePath = defaultProfileImagePath
            };

            var passwordHasher = await _pertukUserManager.UpdatePasswordHash(applicationIdentity, teacherUser.Password);
            if (!passwordHasher.Succeeded) throw new PertukApiException();

            var teacherIdentity = new TeacherUsers
            {
                UserId = applicationIdentity.Id,
                User = applicationIdentity,
                ForStudent = teacherUser.ForStudent,
                UniversityName = teacherUser.UniversityName,
                DepartmentOf = teacherUser.DepartmentOf,
                AcademicOf = teacherUser.AcademicOf,
                Subject = teacherUser.Subject,
                YearsOfExperience = teacherUser.YearsOfExperience
            };

            var result = await _pertukUserManager.CreateTeacher(teacherIdentity);

            if (result == EntityState.Added)
            {
                try { await GenerateAndSendEmailConfirmationLink(applicationIdentity); } catch (Exception) { throw new PertukApiException(); };

                var token = _tokenService.GenerateToken(applicationIdentity);

                return new AuthenticationResponseModel
                {
                    Message = "User created successfully",
                    IsSuccess = true,
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
                        IsSuccess = true,
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
                IsSuccess = true,
                Message = "User Logged In With Facebook!",
                Token = loginToken
            };
        }

        #endregion

        public async Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel)
        {
            if (loginRequestModel == null) throw new PertukApiException(BaseErrorResponseMessages.User.EnterUserDetail);

            var getUserDetail = await _pertukUserManager.GetUserDetailByNameAsync(loginRequestModel.Username);
            if (getUserDetail == null) throw new PertukApiException(BaseErrorResponseMessages.User.UserNotFound);

            _pertukUserManager.CheckUserBanAndDeletion(getUserDetail.Id);

            var checkUserPassword = await _pertukUserManager.CheckPasswordAsync(getUserDetail, loginRequestModel.Password);
            if (!checkUserPassword) throw new PertukApiException(BaseErrorResponseMessages.Password.Invalid);

            var token = _tokenService.GenerateToken(getUserDetail);
            return new AuthenticationResponseModel
            {
                IsSuccess = true,
                Message = "User Logged In",
                Token = token
            };

            #region OLD
            //var isUserStudent = FindStudentUserById(getUserDetail.Id);
            //if (isUserStudent)
            //{
            //    var studentUserToken = _tokenService.GenerateToken(getUserDetail);
            //    return new AuthenticationResponseModel
            //    {
            //        IsSuccess = true,
            //        Token = studentUserToken,
            //        Message = "User Logged In as Student"
            //    };
            //}

            //var isUserTeacher = FindTeacherById(getUserDetail.Id);
            //if (isUserTeacher)
            //{
            //    var teacherUserToken = _tokenService.GenerateToken(getUserDetail);
            //    return new AuthenticationResponseModel
            //    {
            //        IsSuccess = true,
            //        Token = teacherUserToken,
            //        Message = "User Logged In as Teacher"
            //    };
            //}
            #endregion

            throw new PertukApiException();
        }

        public async Task<AuthenticationResponseModel> ConfirmEmailAsync(ConfirmEmailRequestModel confirmEmailRequest)
        {
            if (confirmEmailRequest == null) throw new PertukApiException(BaseErrorResponseMessages.Defaults.EnterDetail);

            if (string.IsNullOrEmpty(confirmEmailRequest.DigitCode) && string.IsNullOrWhiteSpace(confirmEmailRequest.DigitCode)) throw new PertukApiException(BaseErrorResponseMessages.Defaults.InvalidDigitCode);

            var userDetail = await _pertukUserManager.FindByIdAsync(confirmEmailRequest.UserId);
            if (userDetail == null) throw new PertukApiException(BaseErrorResponseMessages.User.UserNotFound);

            var isEmailConfirmed = await _pertukUserManager.IsEmailConfirmedAsync(userDetail);
            if (isEmailConfirmed) throw new PertukApiException(BaseErrorResponseMessages.Email.EmailAlreadyConfirmed);

            var confirmEmailResult = await _pertukUserManager.ConfirmEmailWithDigitCodeAsync(userDetail, confirmEmailRequest.DigitCode);

            if (confirmEmailResult.Succeeded)
            {
                return new AuthenticationResponseModel
                {
                    IsSuccess = true,
                    Message = "Email successfully confirmed!"
                };
            }

            throw new PertukApiException();
        }

        public async Task<AuthenticationResponseModel> SendEmailConfirmation(string userId)
        {
            var userDetail = await _pertukUserManager.FindByIdAsync(userId);

            if (userDetail == null) throw new PertukApiException(BaseErrorResponseMessages.User.UserNotFound);

            await GenerateAndSendEmailConfirmationLink(userDetail);

            return new AuthenticationResponseModel
            {
                IsSuccess = true,
                Message = "Mail successfully sent to your Email address, Please check your Email inbox!"
            };
        }

        public async Task<AuthenticationResponseModel> SendResetPasswordLink(ForgotPasswordRequestModel forgotPasswordRequest)
        {
            var userDetail = await _pertukUserManager.FindByEmailAsync(forgotPasswordRequest.Email);

            if (userDetail == null) throw new PertukApiException(BaseErrorResponseMessages.User.UserNotFound);

            await GenerateAndSendResetPasswordLink(userDetail);

            return new AuthenticationResponseModel
            {
                IsSuccess = true,
                Message = "Mail successfully sent to your Email address, Please check your Email inbox!"
            };
        }

        public async Task<AuthenticationResponseModel> ResetPassword(ResetPasswordRequestModel resetPasswordRequest)
        {
            if (string.IsNullOrEmpty(resetPasswordRequest.Email) && string.IsNullOrWhiteSpace(resetPasswordRequest.Email) || string.IsNullOrEmpty(resetPasswordRequest.DigitCode) && string.IsNullOrWhiteSpace(resetPasswordRequest.DigitCode)) throw new PertukApiException(BaseErrorResponseMessages.Defaults.InvalidDetails);

            var userDetail = await _pertukUserManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (userDetail == null) throw new PertukApiException(BaseErrorResponseMessages.User.UserNotFound);

            var resetPasswordResult = await _pertukUserManager.ResetPasswordWithDigitCodeAsync(userDetail, resetPasswordRequest.DigitCode, resetPasswordRequest.NewPassword);

            if (resetPasswordResult.Succeeded)
            {
                return new AuthenticationResponseModel
                {
                    IsSuccess = true,
                    Message = "Password successfully changed"
                };
            }

            throw new PertukApiException();
        }

        #region Private Functions

        private async Task GenerateAndSendEmailConfirmationLink(ApplicationUser userDetail)
        {
            var confirmationDigitCode = await _pertukUserManager.GenerateDigitCodeForEmailConfirmationAsync(userDetail);

            await _emailSender.SendEmailConfirmation(confirmationDigitCode, userDetail.Email, userDetail.Fullname);
        }

        private async Task GenerateAndSendResetPasswordLink(ApplicationUser userDetail)
        {
            var resetPasswordDigitCode = await _pertukUserManager.GenerateDigitCodeForEmailConfirmationAsync(userDetail);

            await _emailSender.SendResetPassword(resetPasswordDigitCode, userDetail.Email, userDetail.Fullname);
        }

        private async Task CheckAndVerifyUserDetailForRegistering(string email, string username = null)
        {
            var isEmailExist = await _pertukUserManager.FindByEmailAsync(email);
            if (isEmailExist != null)
            {
                _pertukUserManager.CheckUserBanAndDeletion(isEmailExist.Id);
                throw new PertukApiException(BaseErrorResponseMessages.Email.EmailExist);
            }

            if (username != null)
            {
                var isUsernameExist = await _pertukUserManager.FindByNameAsync(username);
                if (isUsernameExist != null)
                {
                    _pertukUserManager.CheckUserBanAndDeletion(isUsernameExist.Id);
                    throw new PertukApiException(BaseErrorResponseMessages.Username.UsernameExist);
                }
            }
        }

        #endregion
    }
}