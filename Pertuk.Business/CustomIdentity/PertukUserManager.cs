using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Pertuk.Business.CustomIdentity.Providers;
using Pertuk.Business.CustomIdentity.Statics;
using Pertuk.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Pertuk.Business.CustomIdentity
{
    public class PertukUserManager : UserManager<ApplicationUser>
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private readonly DigitTokenProvider _digitTokenProvider;
        public PertukUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger, DigitTokenProvider digitTokenProvider)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _digitTokenProvider = digitTokenProvider;
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

        #endregion
    }
}
