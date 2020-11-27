using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class StudentUsersRepository : BaseRepository<StudentUsers, string>, IStudentUsersRepository
    {
        public StudentUsersRepository(PertukDbContext pertukDbContext) : base(pertukDbContext)
        {
        }
    }
}