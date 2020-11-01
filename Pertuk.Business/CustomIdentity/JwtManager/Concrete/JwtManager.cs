using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pertuk.Business.CustomIdentity.JwtManager.Abstract;
using Pertuk.Business.Options;
using Pertuk.Common.Exceptions;
using Pertuk.Common.MiddleWare.Statics;
using Pertuk.Contracts.V1.Requests.Auth;
using Pertuk.Entities.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pertuk.Business.CustomIdentity.JwtManager.Concrete
{
    public class JwtManager : IJwtManager
    {
        private readonly JwtOption _jwtOptions;
        private readonly MediaOptions _mediaOptions;
        private readonly PertukUserManager _pertukUserManager;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtManager(
            IOptions<JwtOption> jwtOptions,
            IOptions<MediaOptions> options,
            PertukUserManager pertukUserManager,
            TokenValidationParameters tokenValidationParameters)
        {
            _jwtOptions = jwtOptions.Value;
            _mediaOptions = options.Value;
            _pertukUserManager = pertukUserManager;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<JwtManagerResponse> GenerateToken(ApplicationUser applicationUser)
        {
            var token = GenerateJwtToken(applicationUser);
            var refreshToken = await GenerateRefreshToken(applicationUser, "Pertuk");
            return new JwtManagerResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken
            };

        }

        public async Task<JwtManagerResponse> RefreshTokenAsync(RefreshTokenRequestModel refreshTokenRequest)
        {
            if (refreshTokenRequest == null) throw new PertukApiException("Please provide required information!");

            var validatedToken = GetPrincipalFromToken(refreshTokenRequest.AccessToken);

            if (validatedToken == null) throw new PertukApiException("Invalid Access Token!");

            string userEmail = validatedToken.Claims.FirstOrDefault(x => x.Type == UserClaimTypes.Email).Value;

            var userDetails = await _pertukUserManager.FindByEmailAsync(userEmail);

            if (userDetails == null) throw new PertukApiException("User not found!");

            var validateRefreshToken = await _pertukUserManager.VerifyUserTokenAsync(userDetails, "Pertuk", "RefreshToken", refreshTokenRequest.RefreshToken);

            if (!validateRefreshToken)
            {
                // Invalid Refresh Token!
                throw new PertukApiException("This refresh token is invalid!");
            }

            var newRefreshToken = await GenerateRefreshToken(userDetails, "Pertuk");
            var newAccessToken = GenerateJwtToken(userDetails);

            return new JwtManagerResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        #region Private Functions

        private Claim[] GenerateClaims(ApplicationUser applicationUser)
        {
            List<Claim> userClaims = new List<Claim>();

            if (applicationUser.RegisterFrom == "Pertuk")
            {
                // Use Pertuk Own CDN Service..
                applicationUser.ProfileImagePath = _mediaOptions.SitePath + applicationUser.ProfileImagePath;
            }

            var baseUserClaims = new[]
            {
                new Claim(UserClaimTypes.UserId, applicationUser.Id),
                new Claim(UserClaimTypes.Fullname, applicationUser.Fullname ?? "Pertuk User"),
                new Claim(UserClaimTypes.Username, applicationUser.UserName),
                new Claim(UserClaimTypes.Email, applicationUser.Email),
                new Claim(UserClaimTypes.ProfileImagePath, applicationUser.ProfileImagePath),
                new Claim(UserClaimTypes.Points, applicationUser.Points.ToString()),
                new Claim(UserClaimTypes.Score, applicationUser.Score.ToString()),
                new Claim(UserClaimTypes.CreatedDate, applicationUser.CreatedAt.ToString())
            };

            userClaims.AddRange(baseUserClaims);

            if (applicationUser.StudentUsers != null)
            {
                // Student
                var studentUserClaims = new[]
                {
                    new Claim(UserClaimTypes.Department, applicationUser.StudentUsers.Department ?? ""),
                    new Claim(UserClaimTypes.Grade, applicationUser.StudentUsers.Grade.ToString() ?? ""),
                    new Claim(UserClaimTypes.StudentUser, "true")
                };
                userClaims.AddRange(studentUserClaims);
            }
            else if (applicationUser.TeacherUsers != null)
            {
                // Teacher
                var teacherUserClaims = new[]
                {
                    new Claim(UserClaimTypes.ForStudent, applicationUser.TeacherUsers.ForStudent ?? ""),
                    new Claim(UserClaimTypes.UniversityName, applicationUser.TeacherUsers.UniversityName ?? ""),
                    new Claim(UserClaimTypes.AcademicOf, applicationUser.TeacherUsers.AcademicOf ?? ""),
                    new Claim(UserClaimTypes.DepartmentOf, applicationUser.TeacherUsers.DepartmentOf ?? ""),
                    new Claim(UserClaimTypes.Subject, applicationUser.TeacherUsers.Subject ?? ""),
                    new Claim(UserClaimTypes.YearsOfExperience, applicationUser.TeacherUsers.YearsOfExperience.ToString() ?? ""),
                    new Claim(UserClaimTypes.IsVerified, applicationUser.TeacherUsers.IsVerified.ToString() ?? ""),
                    new Claim(UserClaimTypes.Certficates, applicationUser.TeacherUsers.Certficates),
                    new Claim(UserClaimTypes.TeacherUser, "true")
                };
                userClaims.AddRange(teacherUserClaims);
            }
            else
            {
                var baseOtherClaims = new[]
                {
                    new Claim(UserClaimTypes.StudentUser, "false"),
                    new Claim(UserClaimTypes.TeacherUser, "false")
                };

                userClaims.AddRange(baseOtherClaims);
            }

            return userClaims.ToArray();
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var userClaims = GenerateClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(_jwtOptions.TokenLifeTime.TotalMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenAsString;
        }

        private async Task<string> GenerateRefreshToken(ApplicationUser user, string provider)
        {
            string getUserAuthenticationToken = await _pertukUserManager.GetAuthenticationTokenAsync(user, provider, "RefreshToken");

            if (getUserAuthenticationToken != null)
            {
                IdentityResult removeUserAuthenticationTokenResult = await _pertukUserManager.RemoveAuthenticationTokenAsync(user, provider, "RefreshToken");
            }

            var newUserRefreshToken = await _pertukUserManager.GenerateUserTokenAsync(user, provider, "RefreshToken");
            var saveUserRefreshToken = await _pertukUserManager.SetAuthenticationTokenAsync(user, provider, "RefreshToken", newUserRefreshToken);

            if (!saveUserRefreshToken.Succeeded)
            {
                //TODO: Throw Exception?? Try Again?? Return Refresh??
            }

            return newUserRefreshToken;
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameter = _tokenValidationParameters.Clone();
                tokenValidationParameter.ValidateLifetime = false;

                var principals = jwtHandler.ValidateToken(token, tokenValidationParameter, out SecurityToken validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principals;

            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

    }
}