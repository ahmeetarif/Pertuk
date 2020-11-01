using Pertuk.Business.Externals.Contracts;
using System.Threading.Tasks;

namespace Pertuk.Business.Externals.Managers.Abstract
{
    public interface IFacebookAuthenticationManager
    {
        Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken);

        Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken);
    }
}