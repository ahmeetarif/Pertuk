using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.MiddleWare.Statics;
using Pertuk.Dto.EntitiesDtos;
using Pertuk.Entities.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
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

        public TokenService(IConfiguration configuration, IOptions<MediaOptions> options)
        {
            _configuration = configuration;
            JwtOptions = new JwtOption();
            _configuration.GetSection(nameof(JwtOption)).Bind(JwtOptions);
            _mediaOptions = options.Value;
        }

        public string CreateStudentUserToken(ApplicationUser studentUsers)
        {
            var userClaims = GenerateClaimsForStudentUser(studentUsers);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.Secret));

            var token = new JwtSecurityToken(
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: userClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenAsString;
        }

        public string CreateTeacherUserToken(ApplicationUser teacherUsers)
        {
            var userClaims = GenerateClaimsForTeacherUser(teacherUsers);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.Secret));

            var token = new JwtSecurityToken(
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: userClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenAsString;
        }

        #region Private Functions

        private Claim[] GenerateClaimsForStudentUser(ApplicationUser studentUserDetail)
        {
            studentUserDetail.ProfileImagePath = _mediaOptions.SitePath + studentUserDetail.ProfileImagePath;

            var userClaims = new[]
            {
                new Claim(UserClaimTypes.Fullname, studentUserDetail.Fullname),
                new Claim(UserClaimTypes.Username, studentUserDetail.UserName),
                new Claim(UserClaimTypes.Email, studentUserDetail.Email),
                new Claim(UserClaimTypes.UserId, studentUserDetail.Id),
                new Claim(UserClaimTypes.Department, studentUserDetail.Department ?? ""),
                new Claim(UserClaimTypes.Grade, studentUserDetail.StudentUsers.Grade.ToString()),
                new Claim(UserClaimTypes.ProfileImagePath, studentUserDetail.ProfileImagePath),
                new Claim(UserClaimTypes.StudentUser, "true"),
                new Claim(UserClaimTypes.TeacherUser, "false")
            };

            return userClaims;
        }

        private Claim[] GenerateClaimsForTeacherUser(ApplicationUser teacherUserDetail)
        {
            teacherUserDetail.ProfileImagePath = _mediaOptions.SitePath + teacherUserDetail.ProfileImagePath;

            var userClaims = new[]
            {
                new Claim(UserClaimTypes.Fullname, teacherUserDetail.Fullname),
                new Claim(UserClaimTypes.Username, teacherUserDetail.UserName),
                new Claim(UserClaimTypes.Email, teacherUserDetail.Email),
                new Claim(UserClaimTypes.UserId, teacherUserDetail.Id),
                new Claim(UserClaimTypes.Department, teacherUserDetail.Department ?? ""),
                new Claim(UserClaimTypes.ProfileImagePath, teacherUserDetail.ProfileImagePath),
                new Claim(UserClaimTypes.StudentUser, "false"),
                new Claim(UserClaimTypes.TeacherUser, "true")
            };

            return userClaims;
        }

        #endregion
    }
}
