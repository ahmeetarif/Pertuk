using System;
using System.Collections.Generic;

namespace Pertuk.Entities.Models
{
    public partial class Questions
    {
        public Questions()
        {
            Answers = new HashSet<Answers>();
        }

        public long Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual DeletedQuestions DeletedQuestions { get; set; }
        public virtual ICollection<Answers> Answers { get; set; }
    }
}
