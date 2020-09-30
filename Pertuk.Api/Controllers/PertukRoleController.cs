using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pertuk.Business.Services.Abstract;
using Pertuk.Dto.Requests.PertukRole;

namespace Pertuk.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PertukRoleController : Controller
    {
        private readonly IPertukRoleService _pertukRoleService;
        public PertukRoleController(IPertukRoleService pertukRoleService)
        {
            _pertukRoleService = pertukRoleService;
        }

        [HttpPost("AddRoleToDatabase")]
        public async Task<IActionResult> AddRoleToDatabase(CreateRoleRequestModel roleRequestModel)
        {
            await _pertukRoleService.AddRoleToDatabase(roleRequestModel);
            return Ok();
        }
    }
}
