using Pertuk.Core.Entities;

namespace Pertuk.Entities.Models
{
    public partial class TeacherUsers : IEntity
    {
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
