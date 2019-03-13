﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_1_1.ViewModels
{
    public class CourseViewModel
    {
       
        
       
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        public string Description { get; set; }
        public IFormFile FileData { get; set; }
    }
}
