using Pertuk.Contracts.Requests.Auth;
using Pertuk.Contracts.Responses.Auth;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IAuthService
    {
        Task<AuthenticationResponseModel> RegisterAsync(RegisterRequestModel registerRequestModel);
        Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel);
        Task<AuthenticationResponseModel> SendEmailConfirmation(string userId);
        Task<AuthenticationResponseModel> ConfirmEmailAsync(ConfirmEmailRequestModel confirmEmailRequest);
        Task<AuthenticationResponseModel> SendResetPasswordLink(ForgotPasswordRequestModel forgotPasswordRequest);
        Task<AuthenticationResponseModel> ResetPassword(ResetPasswordRequestModel resetPasswordRequest);
        Task<AuthenticationResponseModel> FacebookAuthentication(FacebookAuthRequestModel facebookAuthRequestModel);
    }
}