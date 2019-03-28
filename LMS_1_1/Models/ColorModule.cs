using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_1_1.Models
{
    public class ColorModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string LMSUserId { get; set; }
        public LMSUser LMSUser { get; set; }

   
        public Guid? ModuleId { get; set; }
  
        public Module Module { get; set; }

        [Required]
        public string Color { get; set; }

    }

    public class ColorActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string LMSUserId { get; set; }
        public LMSUser LMSUser { get; set; }

        public Guid? CourseId { get; set; }
        public Course Course { get; set; }


        public Guid? LMSActivityId { get; set; }
        public LMSActivity LMSActivity { get; set; }

        public int? AktivityTypeID { get; set; }

        [Required]
        public string Color { get; set; }

    }
}
