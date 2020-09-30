using Pertuk.Core.Entities;
using System;

namespace Pertuk.Entities.Models
{
    public partial class AnswerReplies : IEntity
    {
        public long Id { get; set; }
        public long AnswerId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Answers Answer { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}