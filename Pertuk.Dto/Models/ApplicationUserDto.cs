using Microsoft.AspNetCore.Identity;
using System;

namespace Pertuk.Dto.Models
{
    public class ApplicationUserDto : IdentityUser
    {
        public string Fullname { get; set; }
        public string ProfileImagePath { get; set; }
        public int Points { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string IsFrom { get; set; }
        public string RegisterFrom { get; set; }
    }
}
