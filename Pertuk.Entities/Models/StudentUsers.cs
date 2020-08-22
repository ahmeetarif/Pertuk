using Pertuk.Core.Entities;

namespace Pertuk.Entities.Models
{
    public partial class StudentUsers : IEntity
    {
        public string UserId { get; set; }
        public int? Grade { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
