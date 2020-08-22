using Pertuk.Core.Entities;
using System;

namespace Pertuk.Entities.Models
{
    public partial class BannedUsers : IEntity
    {
        public string UserId { get; set; }
        public string Reason { get; set; }
        public DateTime BannedAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
