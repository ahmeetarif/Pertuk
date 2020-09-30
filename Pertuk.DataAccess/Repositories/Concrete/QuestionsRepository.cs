using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class QuestionsRepository : BaseRepository<Questions, long>, IQuestionsRepository
    {
        public QuestionsRepository(PertukDbContext pertukDbContext)
            : base(pertukDbContext)
        {

        }
    }
}