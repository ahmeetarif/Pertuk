using Pertuk.Contracts.V1.Requests.Auth;
using Pertuk.Contracts.V1.Responses.Auth;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IAuthService
    {
        Task<AuthenticationResponseModel> RegisterAsync(RegisterRequestModel registerRequestModel);
        Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel);
        Task<AuthenticationResponseModel> SendEmailConfirmationCodeAsync(string userId);
        Task<AuthenticationResponseModel> ConfirmEmailAsync(ConfirmEmailRequestModel confirmEmailRequest);
        Task<AuthenticationResponseModel> SendResetPasswordCodeAsync(ForgotPasswordRequestModel forgotPasswordRequest);
        Task<AuthenticationResponseModel> ResetPasswordAsync(ResetPasswordRequestModel resetPasswordRequest);
        Task<AuthenticationResponseModel> FacebookAuthenticationAsync(FacebookAuthRequestModel facebookAuthRequestModel);
        Task<AuthenticationResponseModel> RefreshTokenAsync(RefreshTokenRequestModel refreshTokenRequest);
        Task<AuthenticationResponseModel> VerifyResetPasswordRecoveryCodeAsync(VerifyResetPasswordRequestModel verifyResetPasswordRequest);
    }
}