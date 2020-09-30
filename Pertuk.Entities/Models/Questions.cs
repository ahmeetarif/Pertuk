using Pertuk.Core.Entities;
using System;
using System.Collections.Generic;

namespace Pertuk.Entities.Models
{
    public partial class Questions : IEntity
    {
        public Questions()
        {
            Answers = new HashSet<Answers>();
        }

        public long Id { get; set; }
        public string UserId { get; set; }
        public string ForStudent { get; set; }
        public string AcademicOf { get; set; }
        public string DepartmentOf { get; set; }
        public string Subject { get; set; }
        public int ForGrade { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual DeletedQuestions DeletedQuestions { get; set; }
        public virtual ICollection<Answers> Answers { get; set; }
    }
}
