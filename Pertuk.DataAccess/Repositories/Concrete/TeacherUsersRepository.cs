using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class TeacherUsersRepository : BaseRepository<TeacherUsers, string>, ITeacherUsersRepository
    {
        public TeacherUsersRepository(PertukDbContext pertukDbContext)
            : base(pertukDbContext)
        {
        }
    }
}