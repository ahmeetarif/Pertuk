using System;

namespace Pertuk.Entities.Models
{
    public partial class BannedUsers
    {
        public long Id { get; set; }
        public string BannedUserId { get; set; }
        public string CreatedAdminId { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ApplicationUser BannedUser { get; set; }
        public virtual ApplicationUser CreatedAdmin { get; set; }
    }
}
