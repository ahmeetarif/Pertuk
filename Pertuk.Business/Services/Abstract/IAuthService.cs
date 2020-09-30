using Pertuk.Dto.Requests.Auth;
using Pertuk.Dto.Responses.Auth;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IAuthService
    {
        //Task<AuthenticationResponseModel> RegisterStudentAsync(StudentUserRegisterRequestModel studentUser);
        //Task<AuthenticationResponseModel> RegisterTeacherAsync(TeacherUserRegisterRequestModel teacherUser);
        Task<AuthenticationResponseModel> RegisterAsync(RegisterRequestModel registerRequestModel);
        Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel);
        Task<AuthenticationResponseModel> SendEmailConfirmation(string userId);
        Task<AuthenticationResponseModel> ConfirmEmailAsync(ConfirmEmailRequestModel confirmEmailRequest);
        Task<AuthenticationResponseModel> SendResetPasswordLink(ForgotPasswordRequestModel forgotPasswordRequest);
        Task<AuthenticationResponseModel> ResetPassword(ResetPasswordRequestModel resetPasswordRequest);
        Task<AuthenticationResponseModel> FacebookAuthentication(FacebookAuthRequestModel facebookAuthRequestModel);
    }
}