using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_1_1.ViewModels
{
    public class CloneFormModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime NewDate { get; set; }
        public string Description { get; set; }
        public IFormFile FileData { get; set; }

        public int CloneTypeId { get; set; }
        public Guid? NewCourseId { get; set; }
    }

    public class CloneModuleModel
    {
        public Guid? Id { get; set; }
  
        public string Name { get; set; }
    
        public DateTime StartDate { get; set; }
     
        public DateTime EndDate { get; set; }
    
        public string Description { get; set; }

        public Guid? NewModuleid { get; set; }

        public Guid CourseId { get; set; }
    }
 public class CloneActivityModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int ActivityTypeId { get; set; }
        public Guid moduleid { get; set; }

        public Guid NewActivityId { get; set; }
    }

     public class CloneDocumentModel
    {

        public string Id { get; set; }
        public string Name { get; set; }

        public DateTime UploadDate { get; set; }
        public string  Description { get; set; }
        public string Path { get; set; }

        public string LMSUserId { get; set; }
  
        public Guid? CourseId { get; set; }
        public Guid? NewCourseId { get; set; }
    
        public Guid? ModuleId { get; set; }
        public Guid? NewModuleId { get; set; }
        
        public Guid? LMSActivityId { get; set; }
        public Guid? NewLMSActivityId { get; set; }
      

        public int DocumentTypeId { get; set; }
    
     
    }
}