using Microsoft.AspNetCore.Http;

namespace Pertuk.Dto.Requests.UserManager
{
    public class ChangeUserDetailRequestModel
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public IFormFile ProfileImage { get; set; }
        public string Grade { get; set; }
        public string Department { get; set; }
        public string ForStudent { get; set; }
        public string UniversityName { get; set; }
        public string AcademicOf { get; set; }
        public string DepartmentOf { get; set; }
        public string Subject { get; set; }
        public int? YearsOfExperience { get; set; }
        public bool? IsVerified { get; set; }
        public string EducationStatus { get; set; }
    }
}