using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.MiddleWare.Statics;
using Pertuk.Entities.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pertuk.Business.Services.Concrete
{
    public class TokenService : ITokenService
    {
        #region Private Variables

        private readonly IConfiguration _configuration;

        #endregion

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(ApplicationUser userInfo)
        {
            var userClaims = GenerateClaims(userInfo);

            var jwtOptions = new JwtOption();
            _configuration.GetSection(nameof(JwtOption)).Bind(jwtOptions);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

            var token = new JwtSecurityToken(
                    issuer: jwtOptions.Issuer,
                    audience: jwtOptions.Audience,
                    claims: userClaims,
                    expires: DateTime.Now.AddDays(10),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenAsString;

        }

        #region Private Functions

        private Claim[] GenerateClaims(ApplicationUser userInfo)
        {
            var userClaims = new[]
            {
                new Claim(UserClaimTypes.Firstname, userInfo.Firstname),
                new Claim(UserClaimTypes.Lastname, userInfo.Lastname),
                new Claim(UserClaimTypes.Username, userInfo.UserName),
                new Claim(UserClaimTypes.Email, userInfo.Email)
            };

            return userClaims;
        }

        #endregion
    }
}
