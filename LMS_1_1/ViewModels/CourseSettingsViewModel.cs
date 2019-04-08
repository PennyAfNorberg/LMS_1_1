using LMS_1_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_1_1.ViewModels
{
    public class CourseSettingsViewModel
    {
        public Guid Id { get; set; }
        public Guid? CourseId { get; set; }


        public DateTime? Date { get; set; }

        public string StartTime { get; set; }
        public string StartLunch { get; set; }
        public string EndLunch { get; set; }
        public string EndTime { get; set; }

        public DateTime ForDate { get; set; }
        public int? N { get; set; }

        public int? M { get; set; }
        public int?  Savek { get; set; }
    }
}
