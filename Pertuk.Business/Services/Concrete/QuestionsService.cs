using Microsoft.EntityFrameworkCore;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Common.MiddleWare;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Dto.Requests.Questions;
using Pertuk.Entities.Models;
using System;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class QuestionsService : IQuestionsService
    {
        private readonly IQuestionsRepository _questionsRepository;
        private readonly CurrentUser _currentUser;
        public QuestionsService(
            IQuestionsRepository questionsRepository,
            CurrentUser currentUser)
        {
            _questionsRepository = questionsRepository;
            _currentUser = currentUser;
        }

        public async Task AddQuestions(AddQuestionRequestModel addQuestionRequestModel)
        {
            if (addQuestionRequestModel == null) throw new PertukApiException("Please provide required information!");

            var currentUserId = _currentUser.Id;

            var result = await _questionsRepository.Add(new Questions
            {
                UserId = currentUserId,
                CreatedAt = DateTime.Now,
                ForGrade = addQuestionRequestModel.ForGrade,
                ForStudent = addQuestionRequestModel.ForStudent,
                Description = addQuestionRequestModel.Description,
                AcademicOf = addQuestionRequestModel.AcademicOf,
                DepartmentOf = addQuestionRequestModel.DepartmentOf,
                ImagePath = "",
                Subject = addQuestionRequestModel.Subject
            });

            if (result == EntityState.Added)
            {
                _questionsRepository.SaveDbChanges();
            }
        }
    }
}
