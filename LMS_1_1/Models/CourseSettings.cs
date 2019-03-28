using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_1_1.Models
{
    public class CourseSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid? CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime? Date { get; set; }

        public string StartTime { get; set; }
        public string StartLunch { get; set; }
        public string EndLunch { get; set; }
        public string EndTime { get; set; }

    }
}
