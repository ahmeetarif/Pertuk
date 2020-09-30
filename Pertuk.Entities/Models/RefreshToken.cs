using Pertuk.Core.Entities;
using System;

namespace Pertuk.Entities.Models
{
    public partial class RefreshToken : IEntity
    {
        public int TokenId { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
