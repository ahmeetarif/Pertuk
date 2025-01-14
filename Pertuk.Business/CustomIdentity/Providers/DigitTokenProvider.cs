﻿using Microsoft.AspNetCore.Identity;
using Pertuk.Business.CustomIdentity.Statics;
using Pertuk.Entities.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Pertuk.Business.CustomIdentity.Providers
{
    public class DigitTokenProvider : PhoneNumberTokenProvider<ApplicationUser>
    {
        public static string PhoneDigit = "PhoneDigit";
        public static string EmailDigit = "EmailDigit";

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        public override async Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            var token = new CustomSecurityToken(await manager.CreateSecurityTokenAsync(user));
            var modifier = await GetUserModifierAsync(purpose, manager, user);
            var code = Rfc6238AuthenticationService.GenerateCode(token, modifier, 6).ToString("D4", CultureInfo.InvariantCulture);

            return code;
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            int code;
            if (!Int32.TryParse(token, out code))
            {
                return false;
            }

            var securityToken = new CustomSecurityToken(await manager.CreateSecurityTokenAsync(user));
            var modifier = await GetUserModifierAsync(purpose, manager, user);
            var valid = Rfc6238AuthenticationService.ValidateCode(securityToken, code, modifier, token.Length);

            return valid;
        }

        public override Task<string> GetUserModifierAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return base.GetUserModifierAsync(purpose, manager, user);
        }
    }
}
