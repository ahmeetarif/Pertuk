﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        public async Task<IActionResult> RegisterStudent([FromBody] StudentUserRegisterRequestModel studentUserModel)
        {
            var response = await _authService.RegisterStudentAsync(studentUserModel);

            return Ok(response);
        }

        [HttpPost("RegisterTeacher")]
        public async Task<IActionResult> RegisterTeacher([FromBody] TeacherUserRegisterRequestModel teacherUserModel)
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
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequestModel confirmEmailRequestModel)
        {
            var response = await _authService.ConfirmEmailAsync(confirmEmailRequestModel);

            return Ok(response);
        }

        [HttpPost("SendEmailConfirmation")]
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
    }
}