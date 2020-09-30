using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class AnswerRepliesRepository : BaseRepository<AnswerReplies, long>, IAnswerRepliesRepository
    {
        public AnswerRepliesRepository(PertukDbContext pertukDbContext)
            : base(pertukDbContext)
        {

        }
    }
}
