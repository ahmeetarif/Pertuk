using System.Threading.Tasks;
using Pertuk.Contracts.V1.Requests.UserManager;
using Pertuk.Contracts.V1.Responses.UserManager;
using Pertuk.Dto.Models;

namespace Pertuk.Business.Services.Abstract
{
    public interface IUserManagerService
    {
        Task<UserManagerResponseModel> SetUserStudentAsync(StudentUserRequestModel studentUserRequest);
        Task<UserManagerResponseModel> SetUserTeacherAsync(TeacherUserRequestModel teacherUserRequest);
        ApplicationUserDto GetUserDetail();
    }
}
