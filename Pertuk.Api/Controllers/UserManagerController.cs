using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pertuk.Business.Services.Abstract;
using Pertuk.Contracts.V1.Requests.UserManager;
using Pertuk.Contracts.V1;

namespace Pertuk.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class UserManagerController : Controller
    {
        private readonly IUserManagerService _userManagerService;

        public UserManagerController(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        [HttpPost(ApiRoutes.UserManager.SetStudentUser)]
        public async Task<IActionResult> SetStudentUser(StudentUserRequestModel studentUserRequestModel)
        {
            var response = await _userManagerService.SetUserStudentAsync(studentUserRequestModel);
            return Ok(response);
        }

        [HttpGet(ApiRoutes.UserManager.GetCurrentDetails)]
        public IActionResult GetCurrentDetails()
        {
            var response = _userManagerService.GetUserDetail();
            return Ok(response);
        }
    }
}