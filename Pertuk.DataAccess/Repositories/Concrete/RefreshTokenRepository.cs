using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken, int>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(PertukDbContext pertukDbContext)
            : base(pertukDbContext)
        {

        }
    }
}
