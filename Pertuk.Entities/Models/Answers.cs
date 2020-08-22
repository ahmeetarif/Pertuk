using System;

namespace Pertuk.Entities.Models
{
    public partial class Answers
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Questions Question { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
