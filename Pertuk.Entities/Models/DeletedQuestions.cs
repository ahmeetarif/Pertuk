using Pertuk.Core.Entities;
using System;

namespace Pertuk.Entities.Models
{
    public partial class DeletedQuestions : IEntity
    {
        public long QuestionId { get; set; }
        public string Reason { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual Questions Question { get; set; }
    }
}
