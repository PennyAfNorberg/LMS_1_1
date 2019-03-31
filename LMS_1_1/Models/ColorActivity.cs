using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS_1_1.Models
{
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
