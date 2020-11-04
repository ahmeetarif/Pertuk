namespace Pertuk.Dto.Models
{
    public class StudentUsersDto
    {
        public string UserId { get; set; }
        public int? Grade { get; set; }
        public string Department { get; set; }

        public virtual ApplicationUserDto ApplicationUser { get; set; }
    }
}