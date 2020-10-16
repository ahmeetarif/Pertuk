using System.Threading.Tasks;
using Pertuk.Contracts.Requests.UserManager;
using Pertuk.Contracts.Responses.UserManager;

namespace Pertuk.Business.Services.Abstract
{
    public interface IUserManagerService
    {
        Task<UserManagerResponseModel> SetUserStudentAsync(StudentUserRequestModel studentUserRequest);
        Task<UserManagerResponseModel> SetUserTeacherAsync(TeacherUserRequestModel teacherUserRequest);
    }
}
