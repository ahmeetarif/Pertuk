using Pertuk.Dto.Requests.Auth;
using Pertuk.Dto.Responses.Auth;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IAuthService
    {
        Task<AuthenticationResponseModel> RegisterAsync(RegisterRequestModel registerRequestModel);
        Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel);
        Task<UserResponseModel> SendEmailConfirmation(string userId);
        Task<UserResponseModel> ConfirmEmailAsync(ConfirmEmailRequestModel confirmEmailRequest);
        Task<UserResponseModel> SendResetPasswordLink(ForgotPasswordRequestModel forgotPasswordRequest);
        Task<UserResponseModel> ResetPassword(ResetPasswordRequestModel resetPasswordRequest);
    }
}