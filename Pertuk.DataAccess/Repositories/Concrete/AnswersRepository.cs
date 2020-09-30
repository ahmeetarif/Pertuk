using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class AnswersRepository : BaseRepository<Answers, long>, IAnswersRepository
    {
        public AnswersRepository(PertukDbContext pertukDbContext)
            : base(pertukDbContext)
        {

        }
    }
}