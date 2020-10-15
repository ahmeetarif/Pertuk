using System;

namespace Pertuk.Dto.Models
{
    public class AnswerRepliesDto
    {
        public long Id { get; set; }
        public long AnswerId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}