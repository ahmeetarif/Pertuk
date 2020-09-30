using Microsoft.AspNetCore.Http;
using System;

namespace Pertuk.Dto.Requests.Questions
{
    public class AddQuestionRequestModel
    {
        public string ForStudent { get; set; }
        public string AcademicOf { get; set; }
        public string DepartmentOf { get; set; }
        public string Subject { get; set; }
        public int ForGrade { get; set; }
        public string Description { get; set; }
        public IFormFile QuestionImage { get; set; }
    }
}
