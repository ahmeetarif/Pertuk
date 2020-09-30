using Pertuk.Core.Entities;
using System;
using System.Collections.Generic;

namespace Pertuk.Entities.Models
{
    public partial class Answers : IEntity
    {
        public Answers()
        {
            AnswerReplies = new HashSet<AnswerReplies>();
        }

        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string UserId { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public bool IsRight { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual Questions Question { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<AnswerReplies> AnswerReplies { get; set; }
    }
}