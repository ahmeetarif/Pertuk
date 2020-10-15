namespace Pertuk.Entities.Models
{
    public class TeacherUsersDto
    {
        public string UserId { get; set; }
        public string ForStudent { get; set; }
        public string UniversityName { get; set; }
        public string AcademicOf { get; set; }
        public string DepartmentOf { get; set; }
        public string Subject { get; set; }
        public string Certficates { get; set; }
        public int? YearsOfExperience { get; set; }
        public bool IsVerified { get; set; }
    }
}
