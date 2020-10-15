using System;

namespace Pertuk.Entities.Models
{
    public class QuestionsDto
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string ForStudent { get; set; }
        public string AcademicOf { get; set; }
        public string DepartmentOf { get; set; }
        public string Subject { get; set; }
        public int ForGrade { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}
