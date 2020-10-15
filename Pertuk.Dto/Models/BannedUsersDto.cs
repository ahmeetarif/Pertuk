using System;

namespace Pertuk.Dto.Models
{
    public class BannedUsersDto
    {
        public string UserId { get; set; }
        public string Reason { get; set; }
        public DateTime BannedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}