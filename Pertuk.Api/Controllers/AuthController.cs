using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Mozilla;
using Pertuk.Business.Services.Abstract;
using Pertuk.Dto.Requests.Auth;
using System.Threading.Tasks;

namespace Pertuk.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        #region Private Variables

        private readonly IAuthService _authService;

        #endregion

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Register

        [HttpPost("RegisterStudent")]
        public async Task<IActionResult> RegisterStudent([FromForm] StudentUserRegisterRequestModel studentUserModel)
        {
            var response = await _authService.RegisterStudentAsync(studentUserModel);

            return Ok(response);
        }

        [HttpPost("RegisterTeacher")]
        public async Task<IActionResult> RegisterTeacher([FromForm] TeacherUserRegisterRequestModel teacherUserModel)
        {
            var response = await _authService.RegisterTeacherAsync(teacherUserModel);
            return Ok(response);
        }

        #endregion

        #region Login

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequestModel loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);

            return Ok(response);
        }

        #endregion

        #region Email Confirmation

        [HttpPost("ConfirmEmail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SendEmailConfirmationPolicy")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequestModel confirmEmailRequestModel)
        {
            var response = await _authService.ConfirmEmailAsync(confirmEmailRequestModel);

            return Ok(response);
        }

        [HttpPost("SendEmailConfirmation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SendEmailConfirmationPolicy")]
        public async Task<IActionResult> SendEmailConfirmation([FromBody] string userId)
        {
            var response = await _authService.SendEmailConfirmation(userId);

            return Ok(response);
        }

        #endregion

        #region Reset Password

        [HttpPost("SendResetPassword")]
        public async Task<IActionResult> SendResetPassword([FromBody] ForgotPasswordRequestModel forgotPasswordRequestModel)
        {
            var response = await _authService.SendResetPasswordLink(forgotPasswordRequestModel);

            return Ok(response);
        }

        #endregion

        #region Facebook Auth

        [HttpPost("FacebookAuthentication")]
        public async Task<IActionResult> FacebookAuthentication(FacebookAuthRequestModel facebookAuthRequestModel)
        {
            var response = await _authService.FacebookAuthentication(facebookAuthRequestModel);

            return Ok(response);
        }

        #endregion
    }
}