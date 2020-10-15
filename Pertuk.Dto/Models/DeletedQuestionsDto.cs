using System;

namespace Pertuk.Dto.Models
{
    public class DeletedQuestionsDto
    {
        public long QuestionId { get; set; }
        public string Reason { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}
