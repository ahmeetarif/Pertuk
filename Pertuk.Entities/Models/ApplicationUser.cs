using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Pertuk.Entities.Models
{
    public partial class ApplicationUser : IdentityUser
    {
        public string Fullname { get; set; }
        public string ProfileImagePath { get; set; }
        public int Points { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string IsFrom { get; set; }

        public virtual BannedUsers BannedUsers { get; set; }
        public virtual DeletedUsers DeletedUsers { get; set; }
        public virtual StudentUsers StudentUsers { get; set; }
        public virtual TeacherUsers TeacherUsers { get; set; }
        public virtual ICollection<AnswerReplies> AnswerReplies { get; set; }
        public virtual ICollection<Answers> Answers { get; set; }
        public virtual ICollection<Questions> Questions { get; set; }
        public virtual ICollection<RefreshToken> RefreshToken { get; set; }
    }
}