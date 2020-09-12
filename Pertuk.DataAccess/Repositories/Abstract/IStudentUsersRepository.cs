using Microsoft.EntityFrameworkCore;
using Pertuk.DataAccess.BaseRepository;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Abstract
{
    public interface IStudentUsersRepository : IBaseRepository<StudentUsers, string>
    {
        EntityState AddUsersAndStudent(StudentUsers entity);
        EntityState AddStudent(StudentUsers studentUsers);
    }
}
