namespace Pertuk.Contracts.Requests.UserManager
{
    public class TeacherUserRequestModel
    {
        public string UserId { get; set; }
        public string ForStudent { get; set; }
        public string UniversityName { get; set; }
        public string AcademicOf { get; set; }
        public string DepartmentOf { get; set; }
        public string Subject { get; set; }
        public string Cerficates { get; set; }
        public int YearsOfExperience { get; set; }
    }
}