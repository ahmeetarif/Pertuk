using Microsoft.EntityFrameworkCore;
using Pertuk.DataAccess.BaseRepository;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Abstract
{
    public interface ITeacherUsersRepository : IBaseRepository<TeacherUsers, string>
    {
        EntityState AddTeacher(TeacherUsers teacherUsers);
        EntityState AddUsersAndTeacher(TeacherUsers entity);
    }
}
