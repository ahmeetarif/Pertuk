using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class DeletedUsersRepository : BaseRepository<DeletedUsers, string>, IDeletedUsersRepository
    {
        public DeletedUsersRepository(PertukDbContext pertukDbContext)
            : base(pertukDbContext)
        {

        }
    }
}