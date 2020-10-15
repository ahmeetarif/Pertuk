using System;

namespace Pertuk.Dto.Models
{
    public class AnswersDto
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string UserId { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public bool IsRight { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}