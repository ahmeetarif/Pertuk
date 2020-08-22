using Pertuk.Core.Entities;
using System;

namespace Pertuk.Entities.Models
{
    public partial class DeletedUsers : IEntity
    {
        public string UserId { get; set; }
        public string Reason { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
