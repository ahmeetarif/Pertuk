using Pertuk.Contracts.V1.Requests.Auth;
using Pertuk.Entities.Models;
using System.Threading.Tasks;

namespace Pertuk.Business.CustomIdentity.JwtManager.Abstract
{
    public interface IJwtManager
    {
        Task<JwtManagerResponse> GenerateToken(ApplicationUser applicationUser);
        Task<JwtManagerResponse> RefreshTokenAsync(RefreshTokenRequestModel refreshTokenRequest);
    }
}