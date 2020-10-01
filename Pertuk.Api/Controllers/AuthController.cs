using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Pertuk.Business.Services.Abstract;
using Pertuk.Contracts.V1;
using Pertuk.Dto.Requests.Auth;
using System.Threading.Tasks;

namespace Pertuk.Api.Controllers
{
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Register Student and Teacher Methods

        //[HttpPost(ApiRoutes.Auth.RegisterStudent)]
        //public async Task<IActionResult> RegisterStudent([FromForm] StudentUserRegisterRequestModel studentUserModel)
        //{
        //    var response = await _authService.RegisterStudentAsync(studentUserModel);

        //    return Ok(response);
        //}

        //[HttpPost(ApiRoutes.Auth.RegisterTeacher)]
        //public async Task<IActionResult> RegisterTeacher([FromForm] TeacherUserRegisterRequestModel teacherUserModel)
        //{
        //    var response = await _authService.RegisterTeacherAsync(teacherUserModel);
        //    return Ok(response);
        //}

        #endregion

        #region Register

        [HttpPost(ApiRoutes.Auth.Register)]
        public async Task<IActionResult> Register([FromForm] RegisterRequestModel registerRequestModel)
        {
            var response = await _authService.RegisterAsync(registerRequestModel);
            return Ok(response);
        }

        #endregion

        #region Login

        [HttpPost(ApiRoutes.Auth.Login)]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);

            return Ok(response);
        }

        #endregion

        #region Email Confirmation

        [HttpPost(ApiRoutes.Auth.ConfirmEmail)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SendEmailConfirmationPolicy")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequestModel confirmEmailRequestModel)
        {
            var response = await _authService.ConfirmEmailAsync(confirmEmailRequestModel);

            return Ok(response);
        }

        [HttpPost(ApiRoutes.Auth.SendEmailConfirmation)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SendEmailConfirmationPolicy")]
        public async Task<IActionResult> SendEmailConfirmation([FromBody] string userId)
        {
            var response = await _authService.SendEmailConfirmation(userId);

            return Ok(response);
        }

        #endregion

        #region Reset Password

        [HttpPost(ApiRoutes.Auth.SendResetPassword)]
        public async Task<IActionResult> SendResetPassword([FromBody] ForgotPasswordRequestModel forgotPasswordRequestModel)
        {
            var response = await _authService.SendResetPasswordLink(forgotPasswordRequestModel);

            return Ok(response);
        }

        [HttpPost(ApiRoutes.Auth.ResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel resetPasswordRequestModel)
        {
            var response = await _authService.ResetPassword(resetPasswordRequestModel);
            return Ok(response);
        }

        #endregion

        #region Facebook Auth

        [HttpPost(ApiRoutes.Auth.FacebookAuthentication)]
        public async Task<IActionResult> FacebookAuthentication(FacebookAuthRequestModel facebookAuthRequestModel)
        {
            var response = await _authService.FacebookAuthentication(facebookAuthRequestModel);

            return Ok(response);
        }

        #endregion
    }
}