using Microsoft.AspNetCore.Identity;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Dto.Requests.PertukRole;
using Pertuk.Dto.Responses.PertukRole;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class PertukRoleService : IPertukRoleService
    {
        private readonly PertukUserManager _pertukUserManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PertukRoleService(PertukUserManager pertukUserManager,
            RoleManager<IdentityRole> roleManager)
        {
            _pertukUserManager = pertukUserManager;
            _roleManager = roleManager;
        }

        public async Task<PertukRoleResponseModel> AddRoleToDatabase(CreateRoleRequestModel createRoleRequest)
        {
            if (createRoleRequest.RoleName == null) throw new PertukApiException("Enter Role Name!");

            var isRoleExist = await _roleManager.FindByNameAsync(createRoleRequest.RoleName);

            if (isRoleExist != null) throw new PertukApiException("Role Already Exist!");

            var identityRole = new IdentityRole
            {
                Name = createRoleRequest.RoleName
            };

            var createRoleResult = await _roleManager.CreateAsync(identityRole);

            if (createRoleResult.Succeeded)
            {
                return new PertukRoleResponseModel
                {
                    Message = "Role added to database!"
                };
            }

            throw new PertukApiException();
        }
    }
}
