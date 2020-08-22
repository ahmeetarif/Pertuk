using Pertuk.DataAccess.BaseRepository;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Abstract
{
    public interface IBannedUsersRepository : IBaseRepository<BannedUsers, string>
    {
    }
}
