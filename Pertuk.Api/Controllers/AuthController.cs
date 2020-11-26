using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pertuk.Business.Services.Abstract;
using Pertuk.Contracts.V1.Requests.Auth;
using Pertuk.Contracts.V1;
using System.Threading.Tasks;

namespace Pertuk.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Register

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Auth.Register)]
        public async Task<IActionResult> Register([FromForm] RegisterRequestModel registerRequestModel)
        {
            var response = await _authService.RegisterAsync(registerRequestModel);
            return Ok(response);
        }

        #endregion

        #region Login

        [AllowAnonymous]
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
            var response = await _authService.SendEmailConfirmationCodeAsync(userId);

            return Ok(response);
        }

        #endregion

        #region Reset Password

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Auth.SendResetPassword)]
        public async Task<IActionResult> SendResetPassword([FromBody] ForgotPasswordRequestModel forgotPasswordRequestModel)
        {
            var response = await _authService.SendResetPasswordCodeAsync(forgotPasswordRequestModel);

            return Ok(response);
        }

        [HttpPost(ApiRoutes.Auth.ResetPassword)]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel resetPasswordRequestModel)
        {
            var response = await _authService.ResetPasswordAsync(resetPasswordRequestModel);
            return Ok(response);
        }

        [HttpPost(ApiRoutes.Auth.VerifyResetPasswordRecoveryCode)]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyResetPasswordRecoveryCode([FromBody] VerifyResetPasswordRequestModel verifyResetPasswordRequestModel)
        {
            var response = await _authService.VerifyResetPasswordRecoveryCodeAsync(verifyResetPasswordRequestModel);
            return Ok(response);
        }

        #endregion

        #region Facebook Auth

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Auth.FacebookAuthentication)]
        public async Task<IActionResult> FacebookAuthentication([FromBody] FacebookAuthRequestModel facebookAuthRequestModel)
        {
            var response = await _authService.FacebookAuthenticationAsync(facebookAuthRequestModel);

            return Ok(response);
        }

        #endregion

        #region Refresh Token

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Auth.RefreshToken)]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestModel refreshTokenRequest)
        {
            var response = await _authService.RefreshTokenAsync(refreshTokenRequest);

            return Ok(response);
        }

        #endregion

    }
}