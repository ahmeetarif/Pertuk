using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.Externals.Contracts;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Common.MiddleWare.Statics;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Dto.Requests.Auth;
using Pertuk.Dto.Responses.Auth;
using Pertuk.Entities.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public readonly JwtOption _jwtOptions;
        private readonly MediaOptions _mediaOptions;

        public TokenService(
            IConfiguration configuration,
            IOptions<MediaOptions> options)
        {
            _configuration = configuration;
            _jwtOptions = new JwtOption();
            _configuration.GetSection(nameof(JwtOption)).Bind(_jwtOptions);
            _mediaOptions = options.Value;
        }

        public string GenerateToken(ApplicationUser applicationUser, FacebookPictureData facebookPictureData = null)
        {
            var userClaims = GenerateClaims(applicationUser, facebookPictureData);

            applicationUser.RefreshToken.Add(GenerateRefreshToken());

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: userClaims,
                expires: DateTime.Now.Add(_jwtOptions.TokenLifeTime),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            return tokenAsString;
        }

        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.Now.AddMonths(6);

            return refreshToken;
        }

        #region Private Functions

        private Claim[] GenerateClaims(ApplicationUser applicationUser, FacebookPictureData facebookPictureData = null)
        {
            List<Claim> userClaims = new List<Claim>();

            if (facebookPictureData == null)
            {
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

            var baseOtherClaims = new[]
            {
                new Claim(UserClaimTypes.StudentUser, "false"),
                new Claim(UserClaimTypes.TeacherUser, "false")
            };

            userClaims.AddRange(baseOtherClaims);

            return userClaims.ToArray();
        }

        #endregion
    }
}
