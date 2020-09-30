using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pertuk.Business.Services.Abstract;
using Pertuk.Dto.Requests.Questions;

namespace Pertuk.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class QuestionsController : Controller
    {
        private readonly IQuestionsService _questionsService;
        public QuestionsController(IQuestionsService questionsService)
        {
            _questionsService = questionsService;
        }

        [HttpPost("AddQuestions")]
        public IActionResult Index([FromForm] AddQuestionRequestModel addQuestionRequestModel)
        {
            _questionsService.AddQuestions(addQuestionRequestModel);
            return Ok();
        }

        [HttpGet("GetUserQuestions")]
        public IActionResult GetUserQuestions(string userId)
        {
            return Ok();
        }
    }
}
