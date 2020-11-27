using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pertuk.Business.CustomIdentity.Providers;
using Pertuk.Business.CustomIdentity.Statics;
using Pertuk.Common.Exceptions;
using Pertuk.DataAccess.UnitOfWork;
using Pertuk.Dto.Models;
using Pertuk.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Pertuk.Business.CustomIdentity
{
    public class PertukUserManager : UserManager<ApplicationUser>
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private readonly DigitTokenProvider _digitTokenProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PertukUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger,
            DigitTokenProvider digitTokenProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _digitTokenProvider = digitTokenProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region User Manager

        public virtual ApplicationUser GetUserDetails(string userId)
        {
            ThrowIfDisposed();
            var userDetails = Users.Include(x => x.StudentUsers)
                .Include(x => x.TeacherUsers)
                .FirstOrDefault(x => x.Id == userId);
            return userDetails;
        }

        public virtual async Task<bool> SetUserStudentAsync(ApplicationUser user, StudentUsersDto studentUsers)
        {
            ThrowIfDisposed();
            if (user == null || studentUsers == null) throw new PertukApiException("Please provide required information!");

            var isStudentUserExist = _unitOfWork.StudentUsers.GetById(user.Id);
            if (isStudentUserExist != null) throw new PertukApiException("You're already a student!");

            var studentIdentity = _mapper.Map<StudentUsers>(studentUsers, options =>
            {
                options.AfterMap((src, dest) => dest.User = user);
            });

            try
            {
                await _unitOfWork.StudentUsers.Add(studentIdentity);
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                throw new PertukApiException();
            }
        }

        public virtual async Task<bool> SetUserTeacherAsync(ApplicationUser user, TeacherUsersDto teacherUsers)
        {
            ThrowIfDisposed();
            if (user == null || teacherUsers == null) throw new PertukApiException("Please provide required information!");

            var isTeacherUserExist = _unitOfWork.TeacherUsers.GetById(user.Id);
            if (isTeacherUserExist != null) throw new PertukApiException("You're already a Teacher!");

            var teacherIdentity = _mapper.Map<TeacherUsers>(teacherUsers, options =>
            {
                options.AfterMap((src, dest) => dest.User = user);
            });

            try
            {
                await _unitOfWork.TeacherUsers.Add(teacherIdentity);
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                throw new PertukApiException();
            }
        }

        #endregion

        #region Email Confirmation

        /// <summary>
        /// Validates an email confirmation digit code is valid and matches the specified user.
        /// </summary>
        /// <param name="user">The user to validate the token against.</param>
        /// <param name="digitCode">The email confirmation token to validate.</param>
        /// <returns>The System.Threading.Tasks.Task that represents the asynchronous operation, containing the Microsoft.AspNetCore.Identity.IdentityResult of the operation.</returns>
        public virtual async Task<IdentityResult> ConfirmEmailWithDigitCodeAsync(ApplicationUser user, string digitCode)
        {
            ThrowIfDisposed();
            var store = GetEmailStore();

            if (user == null) throw new ArgumentNullException(nameof(user));

            if (!await _digitTokenProvider.ValidateAsync(DigitTokenProvider.EmailDigit, digitCode, this, user))
            {
                return IdentityResult.Failed(ErrorDescriber.InvalidToken());
            }

            await store.SetEmailConfirmedAsync(user, true, CancellationToken);

            return await UpdateUserAsync(user);
        }

        /// <summary>
        /// Generates an email confirmation digit code for the specified user.
        /// </summary>
        /// <param name="user">The user to generate an email confirmation digit code for.</param>
        /// <returns>The System.Threading.Tasks.Task that represents the asynchronous operation, an email verification digit code.</returns>
        public virtual async Task<string> GenerateDigitCodeForEmailConfirmationAsync(ApplicationUser user)
        {
            ThrowIfDisposed();
            return await _digitTokenProvider.GenerateAsync(DigitTokenProvider.EmailDigit, this, user);
        }

        #endregion

        #region Reset Password

        /// <summary>
        /// Generates an recovery code for reseting password.
        /// </summary>
        /// <param name="user">The user to generate an Recovery Code!</param>
        /// <returns>The System.Threading.Tasks.Task that represents the asynchronous operation, an Recovery Code.</returns>
        public virtual async Task<string> GenerateRecoveryCodeForResetPasswordAsync(ApplicationUser user)
        {
            ThrowIfDisposed();
            return await _digitTokenProvider.GenerateAsync(DigitTokenProvider.EmailDigit, this, user);
        }

        public virtual async Task<IdentityResult> ResetPasswordWithDigitCodeAsync(ApplicationUser user, string digitCode, string newPassword)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (!await _digitTokenProvider.ValidateAsync(DigitTokenProvider.EmailDigit, digitCode, this, user))
            {
                return IdentityResult.Failed(ErrorDescriber.InvalidToken());
            }

            var result = await UpdatePasswordHash(user, newPassword, validatePassword: true);
            if (!result.Succeeded) return result;

            return await UpdateUserAsync(user);
        }

        public virtual async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            var result = await UpdatePasswordHash(user, newPassword, validatePassword: true);
            if (!result.Succeeded) return result;

            return await UpdateUserAsync(user);
        }

        public virtual async Task<bool> VerifyResetPasswordRecoveryCodeAsync(ApplicationUser user, string digitCode)
        {
            ThrowIfDisposed();
            var emailStore = GetEmailStore();

            if (user == null) throw new ArgumentNullException("User is null!");

            try
            {
                var result = await _digitTokenProvider.ValidateAsync(DigitTokenProvider.EmailDigit, digitCode, this, user);

                return result == true ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion


        public virtual void CheckUserBanAndDeletion(string userId)
        {
            ThrowIfDisposed();
            try
            {
                var isBanned = _unitOfWork.BannedUsers.GetById(userId);
                if (isBanned != null && isBanned.IsActive == true) throw new PertukApiException($"This user was banned on : {isBanned.BannedAt.ToShortDateString()}");

                var isDeleted = _unitOfWork.DeletedUsers.GetById(userId);
                if (isDeleted != null && isDeleted.IsActive == true) throw new PertukApiException($"This user was deleted on : {isDeleted.DeletedAt.ToShortDateString()}");
            }
            catch (Exception)
            {
                throw new PertukApiException();
            }
        }

        public virtual async Task<ApplicationUser> GetUserDetailByEmailAsync(string email)
        {
            ThrowIfDisposed();
            var store = GetEmailStore();

            email = KeyNormalizer.NormalizeEmail(email);

            var userDetail = await store.FindByEmailAsync(email, CancellationToken);

            if (userDetail == null) throw new PertukApiException("User not found!");

            var isUserStudent = _unitOfWork.StudentUsers.GetById(userDetail.Id);
            if (isUserStudent != null)
            {
                // Student
                userDetail.StudentUsers = isUserStudent;
                return userDetail;
            }

            var isUserTeacher = _unitOfWork.TeacherUsers.GetById(userDetail.Id);
            if (isUserTeacher != null)
            {
                // Teacher
                userDetail.TeacherUsers = isUserTeacher;
                return userDetail;
            }

            return userDetail;
        }

        public virtual async Task<ApplicationUser> GetUserDetailByNameAsync(string username)
        {
            ThrowIfDisposed();
            username = KeyNormalizer.NormalizeName(username);

            var userDetail = await Store.FindByNameAsync(username, CancellationToken);

            if (userDetail == null) throw new PertukApiException("User not found!");

            var isUserStudent = _unitOfWork.StudentUsers.GetById(userDetail.Id);
            if (isUserStudent != null)
            {
                // Student
                userDetail.StudentUsers = isUserStudent;
                return userDetail;
            }

            var isUserTeacher = _unitOfWork.TeacherUsers.GetById(userDetail.Id);
            if (isUserTeacher != null)
            {
                // Teacher
                userDetail.TeacherUsers = isUserTeacher;
                return userDetail;
            }

            return userDetail;
        }

        #region Private Functions

        private IUserEmailStore<ApplicationUser> GetEmailStore(bool throwOnFail = true)
        {
            var cast = Store as IUserEmailStore<ApplicationUser>;
            if (throwOnFail && cast == null)
            {
                throw new NotSupportedException();
            }
            return cast;
        }

        private async Task UpdateSecurityStampInternal(ApplicationUser user)
        {
            if (SupportsUserSecurityStamp)
            {
                await GetSecurityStore().SetSecurityStampAsync(user, NewSecurityStamp(), CancellationToken);
            }
        }

        private IUserSecurityStampStore<ApplicationUser> GetSecurityStore()
        {
            var cast = Store as IUserSecurityStampStore<ApplicationUser>;
            if (cast == null)
            {
                throw new NotSupportedException();
            }
            return cast;
        }

        private static string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
            _rng.GetBytes(bytes);
            return Base32.ToBase32(bytes);
        }

        #endregion
    }
}