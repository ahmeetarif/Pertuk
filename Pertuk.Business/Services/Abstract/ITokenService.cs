using Pertuk.Entities.Models;

namespace Pertuk.Business.Services.Abstract
{
    public interface ITokenService
    {
        string CreateStudentUserToken(ApplicationUser studentUsersDto);
        string CreateTeacherUserToken(ApplicationUser teacherUsers);
    }
}
