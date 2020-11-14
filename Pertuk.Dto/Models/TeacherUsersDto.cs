namespace Pertuk.Dto.Models
{
    public class TeacherUsersDto
    {
        public string ForStudent { get; set; }
        public string UniversityName { get; set; }
        public string AcademicOf { get; set; }
        public string DepartmentOf { get; set; }
        public string Subject { get; set; }
        public string Certficates { get; set; }
        public int? YearsOfExperience { get; set; }
        public bool IsVerified { get; set; }

        public virtual ApplicationUserDto ApplicationUser { get; set; }
    }
}