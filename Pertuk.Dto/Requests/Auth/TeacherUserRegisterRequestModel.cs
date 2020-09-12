using Pertuk.Core.Dtos;

namespace Pertuk.Dto.Requests.Auth
{
    public class TeacherUserRegisterRequestModel : BaseRegisterDto
    {
        public string ForStudent { get; set; }
        public string UniversityName { get; set; }
        public string AcademicOf { get; set; }
        public string DepartmentOf { get; set; }
        public string Subject { get; set; }
        public int? YearsOfExperience { get; set; }
    }
}