﻿using System;

namespace Pertuk.Entities.Models
{
    public partial class DeletedQuestions
    {
        public long QuestionId { get; set; }
        public string Reason { get; set; }
        public DateTime DeletedAt { get; set; }

        public virtual Questions Question { get; set; }
    }
}
