using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pertuk.Business.CustomIdentity.Providers;
using Pertuk.Business.CustomIdentity.Statics;
using Pertuk.Common.Exceptions;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Pertuk.Business.CustomIdentity
{
    public class PertukUserManager : UserManager<ApplicationUser>
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private readonly DigitTokenProvider _digitTokenProvider;
        private readonly IBannedUsersRepository _bannedUsersRepository;
        private readonly IDeletedUsersRepository _deletedUsersRepository;
        private readonly IStudentUsersRepository _studentUsersRepository;
        private readonly ITeacherUsersRepository _teacherUsersRepository;
        public PertukUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger,
            DigitTokenProvider digitTokenProvider,
            IBannedUsersRepository bannedUsersRepository,
            IDeletedUsersRepository deletedUsersRepository,
            IStudentUsersRepository studentUsersRepository,
            ITeacherUsersRepository teacherUsersRepository)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _digitTokenProvider = digitTokenProvider;
            _bannedUsersRepository = bannedUsersRepository;
            _deletedUsersRepository = deletedUsersRepository;
            _studentUsersRepository = studentUsersRepository;
            _teacherUsersRepository = teacherUsersRepository;
        }

        /// <summary>
        /// Validates that an email confirmation digit code is valid and matches the specified user.
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

        public virtual async Task<IdentityResult> UpdatePasswordHash(ApplicationUser user, string password)
        {
            ThrowIfDisposed();
            var passwordStore = GetPasswordStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var hash = await passwordStore.GetPasswordHashAsync(user, CancellationToken);
            if (hash != null)
            {
                return IdentityResult.Failed(ErrorDescriber.UserAlreadyHasPassword());
            }
            var result = await UpdatePasswordHash(passwordStore, user, password);
            return result;
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

        public virtual void CheckUserBanAndDeletion(string userId)
        {
            try
            {
                var isBanned = _bannedUsersRepository.GetById(userId);
                if (isBanned != null && isBanned.IsActive == true) throw new PertukApiException($"This user was banned on : {isBanned.BannedAt.ToShortDateString()}");

                var isDeleted = _deletedUsersRepository.GetById(userId);
                if (isDeleted != null && isDeleted.IsActive == true) throw new PertukApiException($"This user was deleted on : {isDeleted.DeletedAt.ToShortDateString()}");
            }
            catch (Exception)
            {
                throw new PertukApiException();
            }
        }

        public virtual async Task<EntityState> CreateStudent(StudentUsers studentUsers)
        {
            await UpdateNormalizedEmailAsync(studentUsers.User);
            await UpdateNormalizedUserNameAsync(studentUsers.User);

            return _studentUsersRepository.AddUsersAndStudent(studentUsers);
        }

        public virtual async Task<EntityState> CreateTeacher(TeacherUsers teacherUsers)
        {
            await UpdateNormalizedEmailAsync(teacherUsers.User);
            await UpdateNormalizedUserNameAsync(teacherUsers.User);

            return _teacherUsersRepository.AddUsersAndTeacher(teacherUsers);
        }

        public virtual async Task<ApplicationUser> GetUserDetailByEmailAsync(string email)
        {
            var userDetail = await this.FindByEmailAsync(email);

            if (userDetail == null)
            {
                return null;
            }

            var isUserStudent = _studentUsersRepository.GetById(userDetail.Id);
            if (isUserStudent != null)
            {
                // Student
                userDetail.StudentUsers = isUserStudent;
                return userDetail;
            }

            var isUserTeacher = _teacherUsersRepository.GetById(userDetail.Id);
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
            username = KeyNormalizer.NormalizeName(username);

            var userDetail = await Store.FindByNameAsync(username, CancellationToken);

            var isUserStudent = _studentUsersRepository.GetById(userDetail.Id);
            if (isUserStudent != null)
            {
                // Student
                userDetail.StudentUsers = isUserStudent;
                return userDetail;
            }

            var isUserTeacher = _teacherUsersRepository.GetById(userDetail.Id);
            if (isUserTeacher != null)
            {
                // Teacher
                userDetail.TeacherUsers = isUserTeacher;
                return userDetail;
            }

            throw new PertukApiException();
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

        private IUserPasswordStore<ApplicationUser> GetPasswordStore()
        {
            var cast = Store as IUserPasswordStore<ApplicationUser>;
            if (cast == null)
            {
                throw new NotSupportedException();
            }
            return cast;
        }

        private async Task<IdentityResult> UpdatePasswordHash(IUserPasswordStore<ApplicationUser> passwordStore,
            ApplicationUser user, string newPassword, bool validatePassword = true)
        {
            if (validatePassword)
            {
                var validate = await ValidatePasswordAsync(user, newPassword);
                if (!validate.Succeeded)
                {
                    return validate;
                }
            }
            var hash = newPassword != null ? PasswordHasher.HashPassword(user, newPassword) : null;
            await passwordStore.SetPasswordHashAsync(user, hash, CancellationToken);
            await UpdateSecurityStampInternal(user);
            return IdentityResult.Success;
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

        private IUserLockoutStore<ApplicationUser> GetUserLockoutStore()
        {
            var cast = Store as IUserLockoutStore<ApplicationUser>;
            if (cast == null)
            {
                throw new NotSupportedException();
            }
            return cast;
        }

        #endregion
    }
}
