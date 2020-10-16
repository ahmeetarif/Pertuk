using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pertuk.Business.Services.Abstract;
using Pertuk.Contracts.Requests.UserManager;
using Pertuk.Contracts.V1;

namespace Pertuk.Api.Controllers
{
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
    }
}