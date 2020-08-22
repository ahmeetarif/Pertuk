using Pertuk.Dto.Requests.Auth;
using Pertuk.Dto.Responses.Auth;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IAuthService
    {
        Task<AuthenticationResponseModel> RegisterStudentAsync(StudentUserRegisterRequestModel studentUser);
        Task<AuthenticationResponseModel> RegisterTeacherAsync(TeacherUserRegisterRequestModel teacherUser);
        Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel loginRequestModel);
        Task<UserResponseModel> SendEmailConfirmation(string userId);
        Task<AuthenticationResponseModel> ConfirmEmailAsync(ConfirmEmailRequestModel confirmEmailRequest);
        Task<UserResponseModel> SendResetPasswordLink(ForgotPasswordRequestModel forgotPasswordRequest);
        Task<UserResponseModel> ResetPassword(ResetPasswordRequestModel resetPasswordRequest);
    }
}