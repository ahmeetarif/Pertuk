using System;

namespace Pertuk.Dto.Models
{
    public class DeletedUsersDto
    {
        public string UserId { get; set; }
        public string Reason { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}