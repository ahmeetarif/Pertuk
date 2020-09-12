using Pertuk.Core.Entities;

namespace Pertuk.Entities.Models
{
    public partial class TeacherUsers : IEntity
    {
        public string UserId { get; set; }
        public string ForStudent { get; set; }
        public string UniversityName { get; set; }
        public string AcademicOf { get; set; }
        public string DepartmentOf { get; set; }
        public string Subject { get; set; }
        public int? YearsOfExperience { get; set; }
        public bool IsVerified { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
