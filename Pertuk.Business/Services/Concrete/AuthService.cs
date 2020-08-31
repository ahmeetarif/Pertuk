using BunnyCDN.Net.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Pertuk.Business.BunnyCDN;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class AuthService : IAuthService
    {
        #region Private Variables

        private readonly PertukUserManager _pertukUserManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly IStudentUsersRepository _studentUsersRepository;
        private readonly ITeacherUsersRepository _teacherUsersRepository;
        private readonly IBannedUsersRepository _bannedUsersRepository;
        private readonly IDeletedUsersRepository _deletedUsersRepository;
        private readonly BunnyCDNService _bunnyCDNService;
        private readonly MediaOptions _mediaOptions;
        private readonly IConfiguration _configuration;

        #endregion

        public AuthService(ITokenService tokenService,
                            IEmailSender emailSender,
                            IStudentUsersRepository studentUsersRepository,
                            ITeacherUsersRepository teacherUsersRepository,
                            IBannedUsersRepository bannedUsersRepository,
                            IDeletedUsersRepository deletedUsersRepository,
                            PertukUserManager pertukUserManager,
                            BunnyCDNService bunnyCDNService,
                            IConfiguration configuration,
                            IOptions<MediaOptions> mediaOptions)
        {
            _tokenService = tokenService;
            _emailSender = emailSender;
            _studentUsersRepository = studentUsersRepository;
            _teacherUsersRepository = teacherUsersRepository;
            _bannedUsersRepository = bannedUsersRepository;
            _deletedUsersRepository = deletedUsersRepository;
            _pertukUserManager = pertukUserManager;
            _bunnyCDNService = bunnyCDNService;
            _configuration = configuration;
            _mediaOptions = mediaOptions.Value;
        }

        #region Finished Methods

        public async Task<AuthenticationResponseModel> RegisterStudentAsync(StudentUserRegisterRequestModel studentUser)
        {
            if (studentUser == null) throw new PertukApiException(BaseErrorResponseMessages.EnterUserDetail);

            await CheckAndVerifyUserDetailForRegistering(studentUser.Email, studentUser.Username);

            var defaultProfileImagePath = _mediaOptions.EmptyProfilePicture;

            var applicationIdentity = new ApplicationUser
            {
                Fullname = studentUser.Fullname,
                Department = studentUser.Department,
                UserName = studentUser.Username,
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
                User = applicationIdentity
            };

            var result = await _studentUsersRepository.Add(studentIdentity);

            if (result == EntityState.Added)
            {
                try { await GenerateAndSendEmailConfirmationLink(applicationIdentity); } catch (Exception) { throw new PertukApiException(); };
                var token = _tokenService.CreateToken(applicationIdentity);
                return new AuthenticationResponseModel
                {
                    Message = "User Created successfully!",
                    IsSuccess = true,
                    Token = token
                };
            }

            throw new PertukApiException();
        }

        public async Task<AuthenticationResponseModel> RegisterTeacherAsync(TeacherUserRegisterRequestModel teacherUser)
        {
            if (teacherUser == null) throw new PertukApiException(BaseErrorResponseMessages.EnterUserDetail);

            await CheckAndVerifyUserDetailForRegistering(teacherUser.Email, teacherUser.Username);

            var defaultProfileImagePath = _mediaOptions.EmptyProfilePicture;

            var applicationIdentity = new ApplicationUser
            {
                Fullname = teacherUser.Fullname,
                Department = teacherUser.Department,
                UserName = teacherUser.Username,
                Email = teacherUser.Email,
                CreatedAt = DateTime.Now,
                ProfileImagePath = defaultProfileImagePath
            };

            var passwordHasher = await _pertukUserManager.UpdatePasswordHash(applicationIdentity, teacherUser.Password);
            if (!passwordHasher.Succeeded) throw new PertukApiException();

            var teacherIdentity = new TeacherUsers
            {
                UserId = applicationIdentity.Id,
                User = applicationIdentity
            };

            var result = await _teacherUsersRepository.Add(teacherIdentity);

            if (result == EntityState.Added)
            {
                try { await GenerateAndSendEmailConfirmationLink(applicationIdentity); } catch (Exception) { throw new PertukApiException(); };
                var token = _tokenService.CreateToken(applicationIdentity);
                return new AuthenticationResponseModel
                {
                    Message = "User Created successfully!",
                    IsSuccess = true,
                    Token = token
                };
            }

            throw new PertukApiException();
        }

        public async Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel)
        {
            if (loginRequestModel == null) throw new PertukApiException(BaseErrorResponseMessages.EnterUserDetail);

            var getUserDetail = await _pertukUserManager.FindByNameAsync(loginRequestModel.Username);
            if (getUserDetail == null) throw new PertukApiException(BaseErrorResponseMessages.UsernameNotFound);

            CheckUserBan(getUserDetail.Id);
            CheckUserDeleted(getUserDetail.Id);

            var checkUserPassword = await _pertukUserManager.CheckPasswordAsync(getUserDetail, loginRequestModel.Password);
            if (!checkUserPassword) throw new PertukApiException(BaseErrorResponseMessages.InvalidPassword);

            var token = _tokenService.CreateToken(getUserDetail);

            return new AuthenticationResponseModel
            {
                IsSuccess = true,
                Token = token
            };
        }

        #endregion

        public async Task<AuthenticationResponseModel> ConfirmEmailAsync(ConfirmEmailRequestModel confirmEmailRequest)
        {
            if (confirmEmailRequest == null) throw new PertukApiException("Fill detail");

            if (string.IsNullOrEmpty(confirmEmailRequest.DigitCode) && string.IsNullOrWhiteSpace(confirmEmailRequest.DigitCode)) throw new PertukApiException(BaseErrorResponseMessages.InvalidDigitCode);

            var userDetail = await _pertukUserManager.FindByIdAsync(confirmEmailRequest.UserId);
            if (userDetail == null) throw new PertukApiException(BaseErrorResponseMessages.UserNotFound);

            var isEmailConfirmed = await _pertukUserManager.IsEmailConfirmedAsync(userDetail);
            if (isEmailConfirmed) throw new PertukApiException(BaseErrorResponseMessages.EmailAlreadyConfirmed);

            var confirmEmailResult = await _pertukUserManager.ConfirmEmailWithDigitCodeAsync(userDetail, confirmEmailRequest.DigitCode);

            if (confirmEmailResult.Succeeded)
            {
                return new AuthenticationResponseModel
                {
                    IsSuccess = true,
                    Message = BaseErrorResponseMessages.EmailConfirmed
                };
            }

            throw new PertukApiException();
        }

        public async Task<AuthenticationResponseModel> SendEmailConfirmation(string userId)
        {
            var userDetail = await _pertukUserManager.FindByIdAsync(userId);

            if (userDetail == null) throw new PertukApiException(BaseErrorResponseMessages.UserNotFound);

            await GenerateAndSendEmailConfirmationLink(userDetail);

            return new AuthenticationResponseModel
            {
                IsSuccess = true,
                Message = BaseErrorResponseMessages.ConfirmationCodeSentSuccessfully
            };
        }

        public async Task<AuthenticationResponseModel> SendResetPasswordLink(ForgotPasswordRequestModel forgotPasswordRequest)
        {
            var userDetail = await _pertukUserManager.FindByEmailAsync(forgotPasswordRequest.Email);

            if (userDetail == null) throw new PertukApiException(BaseErrorResponseMessages.UserNotFound);

            await GenerateAndSendResetPasswordLink(userDetail);

            return new AuthenticationResponseModel
            {
                IsSuccess = true,
                Message = BaseErrorResponseMessages.ConfirmationCodeSentSuccessfully
            };
        }

        public async Task<AuthenticationResponseModel> ResetPassword(ResetPasswordRequestModel resetPasswordRequest)
        {
            if (string.IsNullOrEmpty(resetPasswordRequest.Email) && string.IsNullOrWhiteSpace(resetPasswordRequest.Email) || string.IsNullOrEmpty(resetPasswordRequest.Token) && string.IsNullOrWhiteSpace(resetPasswordRequest.Token))
                return new AuthenticationResponseModel
                {
                    IsSuccess = false,
                    Message = "Invalid URL!"
                };

            var userDetail = await _pertukUserManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (userDetail == null)
                return new AuthenticationResponseModel
                {
                    IsSuccess = false,
                    Message = $"User with : {resetPasswordRequest.Email} not found!"
                };

            var tokenFromWebDecoded = WebEncoders.Base64UrlDecode(resetPasswordRequest.Token);
            var validToken = Encoding.UTF8.GetString(tokenFromWebDecoded);

            var resetPasswordResult = await _pertukUserManager.ResetPasswordAsync(userDetail, validToken, resetPasswordRequest.NewPassword);

            if (resetPasswordResult.Succeeded)
            {
                return new AuthenticationResponseModel
                {
                    IsSuccess = true,
                    Message = "Reset password successful!"
                };
            }
            else
            {
                return new AuthenticationResponseModel
                {
                    IsSuccess = false,
                    Message = "Error while reseting your password!",
                    Errors = resetPasswordResult.Errors.Select(x => x.Description)
                };
            }
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

        private async Task CheckUserDetail(string email, string username)
        {
            var isEmailExist = await _pertukUserManager.FindByEmailAsync(email) != null;
            if (isEmailExist) throw new PertukApiException(BaseErrorResponseMessages.EmailExist);

            var isUsernameExist = await _pertukUserManager.FindByNameAsync(username) != null;
            if (isUsernameExist) throw new PertukApiException(BaseErrorResponseMessages.UsernameExist);
        }

        private async Task CheckAndVerifyUserDetailForRegistering(string email, string username)
        {
            var isEmailExist = await _pertukUserManager.FindByEmailAsync(email);
            if (isEmailExist != null)
            {
                CheckUserBan(isEmailExist.Id);
                CheckUserDeleted(isEmailExist.Id);
                throw new PertukApiException(BaseErrorResponseMessages.EmailExist);
            }

            var isUsernameExist = await _pertukUserManager.FindByNameAsync(username);
            if (isUsernameExist != null)
            {
                CheckUserBan(isUsernameExist.Id);
                CheckUserDeleted(isUsernameExist.Id);
                throw new PertukApiException(BaseErrorResponseMessages.UsernameExist);
            }
        }

        private void CheckUserDeleted(string userId)
        {
            var deletedUser = _deletedUsersRepository.GetById(userId);
            if (deletedUser != null && deletedUser.IsActive == true)
            {
                throw new PertukApiException($"This user was deleted on : {deletedUser.DeletedAt.ToShortDateString()}");
            }
        }

        private void CheckUserBan(string userId)
        {
            var bannedUser = _bannedUsersRepository.GetById(userId);
            if (bannedUser != null && bannedUser.IsActive == true)
            {
                throw new PertukApiException($"This user was banned on : {bannedUser.BannedAt.ToShortDateString()}");
            }
        }

        //private async Task<string> UploadImage(IFormFile formFile)
        //{
        //    var defaultPath = MediaOptions.StorageZoneName + MediaOptions.PostsDirectoryPath;

        //    var destination = Path.Combine(defaultPath, Guid.NewGuid().ToString());

        //    var newDestination = destination + ".jpg";

        //    using (Stream reader = formFile.OpenReadStream())
        //    {
        //        await _bunnyCDNService.UploadAsync(reader, newDestination);
        //    }

        //    return newDestination;
        //}

        #endregion
    }
}