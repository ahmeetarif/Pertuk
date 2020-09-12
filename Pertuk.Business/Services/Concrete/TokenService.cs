using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pertuk.Business.Externals.Contracts;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.MiddleWare.Statics;
using Pertuk.Entities.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pertuk.Business.Services.Concrete
{
    public class TokenService : ITokenService
    {
        #region Private Variables

        private readonly IConfiguration _configuration;
        public JwtOption JwtOptions { get; set; }
        public MediaOptions _mediaOptions { get; set; }
        #endregion

        public TokenService(
            IConfiguration configuration,
            IOptions<MediaOptions> options)
        {
            _configuration = configuration;
            JwtOptions = new JwtOption();
            _configuration.GetSection(nameof(JwtOption)).Bind(JwtOptions);
            _mediaOptions = options.Value;
        }

        public string GenerateToken(ApplicationUser applicationUser, FacebookPictureData facebookPictureData = null)
        {
            var userClaims = GenerateClaim(applicationUser, facebookPictureData);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.Secret));

            var token = new JwtSecurityToken(
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: userClaims,
                expires: DateTime.Now.Add(JwtOptions.TokenLifeTime),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenAsString;
        }

        #region Private Functions

        private Claim[] GenerateClaim(ApplicationUser applicationUser, FacebookPictureData facebookPictureData = null)
        {
            List<Claim> userClaims = new List<Claim>();

            if (facebookPictureData == null)
            {
                applicationUser.ProfileImagePath = _mediaOptions.SitePath + applicationUser.ProfileImagePath;
            }

            var baseUserClaims = new[]
            {
                new Claim(UserClaimTypes.UserId, applicationUser.Id),
                new Claim(UserClaimTypes.Fullname, applicationUser.Fullname),
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
                    new Claim(UserClaimTypes.Grade, applicationUser.StudentUsers.Grade.ToString()),
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
                    new Claim(UserClaimTypes.TeacherUser, "true")
                };
                userClaims.AddRange(teacherUserClaims);
            }
            return userClaims.ToArray();
        }

        #endregion
    }
}
