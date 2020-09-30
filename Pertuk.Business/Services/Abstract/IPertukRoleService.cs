using Pertuk.Dto.Requests.PertukRole;
using Pertuk.Dto.Responses.PertukRole;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IPertukRoleService
    {
        Task<PertukRoleResponseModel> AddRoleToDatabase(CreateRoleRequestModel createRoleRequest);
    }
}
