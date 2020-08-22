using Pertuk.DataAccess.BaseRepository;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Abstract
{
    public interface IDeletedUsersRepository : IBaseRepository<DeletedUsers, string>
    {
    }
}
