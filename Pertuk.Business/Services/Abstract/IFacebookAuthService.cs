using Pertuk.Business.Externals.Contracts;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IFacebookAuthService
    {
        Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken);

        Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken);
    }
}