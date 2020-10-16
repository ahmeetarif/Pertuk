using System;
using Pertuk.Dto.Models;
using Pertuk.Entities.Models;

namespace Pertuk.Contracts.Responses.UserManager
{
    public class UsersDetailsResponseModel
    {
        // Defaults
        public string ProfileImagePath { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool EmailConfirmed { get; set; }
        public string IsFrom { get; set; }
        public int Score { get; set; }
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; }

        // Student User
        public StudentUsersDto StudentUsers { get; set; }

        // Techer User
        public TeacherUsersDto TeacherUsers { get; set; }
    }
}