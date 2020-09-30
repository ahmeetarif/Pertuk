using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class DeletedQuestionsRepository : BaseRepository<DeletedQuestions, long>, IDeletedQuestionsRepository
    {
        public DeletedQuestionsRepository(PertukDbContext pertukDbContext)
            : base(pertukDbContext)
        {

        }
    }
}