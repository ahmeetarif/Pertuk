using System;

namespace Pertuk.Dto.Models
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Fullname { get; set; }
        public string ProfileImagePath { get; set; }
        public int Points { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string IsFrom { get; set; }
        public string RegisterFrom { get; set; }

        public virtual StudentUsersDto StudentUsers { get; set; }
        public virtual TeacherUsersDto TeacherUsers { get; set; }
    }
}