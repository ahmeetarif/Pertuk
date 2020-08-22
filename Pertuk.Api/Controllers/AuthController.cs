using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestModel registerRequest)
        {
            var response = await _authService.RegisterAsync(registerRequest);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequestModel loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var response = await _authService.ConfirmEmailAsync(new ConfirmEmailRequestModel { UserId = userId, Token = token });

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("SendEmailConfirmation")]
        public async Task<IActionResult> SendEmailConfirmation([FromBody] string userId)
        {
            var response = await _authService.SendEmailConfirmation(userId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> SendResetPasswordLink([FromBody] ForgotPasswordRequestModel forgotPasswordRequest)
        {
            var response = await _authService.SendResetPasswordLink(forgotPasswordRequest);

            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response);
        }

        // TODO : Confirgure Reset Password
        [HttpGet("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestModel resetPasswordRequest)
        {
            var response = await _authService.ResetPassword(resetPasswordRequest);

            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response);
        }
    }
}