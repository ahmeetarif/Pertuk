using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class BannedUsersRepository : BaseRepository<BannedUsers, string>, IBannedUsersRepository
    {
        public BannedUsersRepository(PertukDbContext pertukDbContext)
            : base(pertukDbContext)
        {

        }
    }
}
