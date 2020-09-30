using Pertuk.Dto.Requests.Questions;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IQuestionsService
    {
        Task AddQuestions(AddQuestionRequestModel addQuestionRequestModel);
    }
}
