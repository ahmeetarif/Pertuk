using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Pertuk.Entities.Models
{
    public partial class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            BannedUsersBannedUser = new HashSet<BannedUsers>();
            BannedUsersCreatedAdmin = new HashSet<BannedUsers>();
        }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Grade { get; set; }
        public long FollowersCount { get; set; }
        public long FollowingCount { get; set; }
        public long QuestionCount { get; set; }
        public long AnswerCount { get; set; }
        public long PointCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BannedAt { get; set; }

        public virtual ICollection<BannedUsers> BannedUsersBannedUser { get; set; }
        public virtual ICollection<BannedUsers> BannedUsersCreatedAdmin { get; set; }
    }
}
