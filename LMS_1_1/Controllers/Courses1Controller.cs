using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS_1_1.Data;
using LMS_1_1.Models;
using LMS_1_1.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using LMS_1_1.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlClient;
using AutoMapper;

namespace LMS_1_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Courses1Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;
        private readonly UserManager<LMSUser> _userManager;
        private readonly ILogger<CoursesController> _logger;
        private readonly IProgramRepository _programrepository;
        private readonly IDocumentRepository _documentrepository;

        public Courses1Controller (IProgramRepository programrepository 
            ,IDocumentRepository documentrepository
            , ILogger<CoursesController> logger
            , ApplicationDbContext context
            , IHostingEnvironment environment
            , UserManager<LMSUser> userManager)
        {
            _programrepository = programrepository;
            _documentrepository = documentrepository;
            _logger = logger;
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        // GET: api/Courses1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
           
            return Ok(await _programrepository.GetAllCoursesAsync(false));
        }


        [HttpGet("foruser")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCoursesforuser()
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);


            return Ok(await _programrepository.GetCoursesForUserAsync(user.Id));
        }

        // GET: api/Courses1/5/true course , modules and activites
        // GET: api/Courses1/5/false  just the course
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(string id)
        {
           Guid idG = Guid.Parse(id);
            Course course = await _context.Courses.FindAsync(idG);
            

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }
        
        [HttpGet("All")]
        public async Task<ActionResult<CourseAllViewModel>> GetCourseAll(string id)
        {
            //   Guid idG = Guid.Parse(id);
  
              var  course1 = await _context.Courses
                            .Include(c => c.Modules)
                            .ThenInclude(m => m.LMSActivities)
                            .ThenInclude(a => a.ActivityType)
                            .FirstOrDefaultAsync(c => c.Id.ToString() == id);



            var Modules = new List<ModelAllViewModel>();
            foreach (var Modul in course1.Modules)
            {
                var Activities = new List<ActivityViewModel>();
                foreach (var Actitivity in Modul.LMSActivities)
                {
                    Activities.Add(
                            new ActivityViewModel
                            {
                                Id = Actitivity.Id,
                                Name = Actitivity.Name,
                                StartDate = Actitivity.StartDate,
                                EndDate = Actitivity.EndDate,
                                Description = Actitivity.Description,
                                ActivityType = Actitivity.ActivityType.Name,
                                Name2 = (Guid.NewGuid()).ToString(),
                                isExpanded = ""

                            }

                        );
                }

                Modules.Add(
                     new ModelAllViewModel
                     {
                         Id = Modul.Id,
                         Name = Modul.Name,
                         StartDate = Modul.StartDate,
                         EndDate = Modul.EndDate,
                         Description = Modul.Description,
                         Activities = (ICollection<ActivityViewModel>)Activities,
                         Name2=(Guid.NewGuid()).ToString(),
                        // Name2 = "C" + (i++).ToString(),
                         isExpanded = ""
                     }
                    );
            }

            CourseAllViewModel course = new CourseAllViewModel
            {
                Id = course1.Id,
                Name = course1.Name,
                StartDate = course1.StartDate,
                Description = course1.Description,
                courseImgPath = course1.CourseImgPath,
                Modules = Modules
            };


            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        [HttpGet("CAndM")]
        public async Task<ActionResult<CourseAllViewModel>> GetCourseAndModule(string id)
        {
            //   Guid idG = Guid.Parse(id);
            var course1 = await _context.Courses
                          .Include(c => c.Modules)
                          .FirstOrDefaultAsync(c => c.Id.ToString() == id);



            var Modules = new List<ModelAllViewModel>();
            foreach (var Modul in course1.Modules)
            {
                

                Modules.Add(
                     new ModelAllViewModel
                     {
                         Id = Modul.Id,
                         Name = Modul.Name,
                         StartDate = Modul.StartDate,
                         EndDate = Modul.EndDate,
                         Description = Modul.Description,
                         Activities = null,
                         Name2=(Guid.NewGuid()).ToString(),
                        // Name2 = "C" + (i++).ToString(),
                         isExpanded = ""
                     }
                    );
            }

            CourseAllViewModel course = new CourseAllViewModel
            {
                Id = course1.Id,
                Name = course1.Name,
                StartDate = course1.StartDate,
                Description = course1.Description,
                courseImgPath = course1.CourseImgPath,
                Modules = Modules
            };


            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }
        
        [HttpGet("AfromMid")]
        public async Task<ActionResult<ICollection<ActivityViewModel>>> GetActivitiesFromModulid(string id)
        {
            //   Guid idG = Guid.Parse(id);
            var Activities =await  _context.LMSActivity
                           .Include(a => a.ActivityType)
                          .Where(a => a.ModuleId.ToString() == id)
                          .ToArrayAsync();
                          



            var res = new List<ActivityViewModel>();
            foreach (var activity in Activities)
            {


                res.Add(
                     new ActivityViewModel
                     {
                         Id = activity.Id,
                         Name = activity.Name,
                         StartDate = activity.StartDate,
                         EndDate = activity.EndDate,
                         Description = activity.Description,
                         ActivityType = activity.ActivityType.Name
                     }
                    );
            }


            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        
        [HttpGet("MAndAfromMid")]
        public async Task<ActionResult<ModelAllViewModel>> GetModulesAndActivitiesFromModulid(string id)
        {
            //   Guid idG = Guid.Parse(id);
    
            var Module = await _context.Modules
                            .Include(m => m.LMSActivities)
                           .ThenInclude(a => a.ActivityType)
                          .Where(m => m.Id.ToString() == id)
                          .FirstOrDefaultAsync();




            var res = new List<ActivityViewModel>();
            foreach (var activity in Module.LMSActivities)
            {


                res.Add(
                     new ActivityViewModel
                     {
                         Id = activity.Id,
                         Name = activity.Name,
                         StartDate = activity.StartDate,
                         EndDate = activity.EndDate,
                         Description = activity.Description,
                         ActivityType = activity.ActivityType.Name,
                         Name2 = (Guid.NewGuid()).ToString(),
                         isExpanded = ""
                     }
                    );
            }


            ModelAllViewModel Module1 = new ModelAllViewModel
            {
                Id = Module.Id,
                Name = Module.Name,
                StartDate = Module.StartDate,
                EndDate = Module.EndDate,
                Description = Module.Description,
                Activities = res,
                CourseId=Module.CourseId
            };

            if (Module1 == null)
            {
                return NotFound();
            }

            return Ok(Module1);
        }
        [HttpGet("CloneTypes")]
        public async Task<ActionResult<IEnumerable<CloneType>>> GetCloneTypes()
        {
            return await _context.CloneTypes.ToArrayAsync();
        }

        [HttpPost("Clone"), DisableRequestSizeLimit]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<Course>> Clone([FromForm] CloneFormModel CloneFormModel )
        {



             string userid=  (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
             var res= await _programrepository.CloneCourseAsync(CloneFormModel, userid);

            return Ok(res);

            /*  var tempteacher = await _userManager.GetUsersInRoleAsync("Teacher");
               var allteachers =tempteacher.Select(u => u.Id);


               //CloneFormModel => old courseid, new start, new image may be null
               // get course
               var coursedata = await _context.Courses
                               .Include(c => c.Modules)
                               .ThenInclude(m => m.LMSActivities)
                               .ThenInclude(a => a.ActivityType)


                               .FirstOrDefaultAsync(c => CloneFormModel.Id == c.Id.ToString());
               /// int datediff = ((TimeSpan) (CloneFormModel.NewDate - coursedata.StartDate).Days;
               CloneFormModel.NewDate = CloneFormModel.NewDate;

               DateTime dt = DateTime.Parse(CloneFormModel.NewDate.ToShortDateString());
               DateTime dt1 = DateTime.Parse(coursedata.StartDate.ToShortDateString());

               int noOfDays = (int) dt.Subtract(dt1).TotalDays; // dates to add



               var tmpcourse = new Course
               {

                   Name = CloneFormModel.Name,
                   Description = CloneFormModel.Description,
                   StartDate = CloneFormModel.NewDate,
                   CourseImgPath =(CloneFormModel.FileData==null)?coursedata.CourseImgPath :@"..\assets\img\" + CloneFormModel.FileData.FileName

               };

               // update course name and date
               // add course, Save Old and new courseid
               await _context.Courses.AddRangeAsync(tmpcourse);
               await _context.SaveChangesAsync();
               CloneFormModel.NewCourseId = tmpcourse.Id;

               var cloneModules = new List<CloneModuleModel>();
               var cloneActivities = new List<CloneActivityModel>();
               foreach (var mod in coursedata.Modules)
               {
                   Module tmp2 = new Module
                   {
                       Name = mod.Name
                     ,
                       Description = mod.Description
                     ,
                       StartDate = mod.StartDate.AddDays(noOfDays)
                     ,
                       EndDate = mod.EndDate.AddDays(noOfDays)
                     ,
                       CourseId = CloneFormModel.NewCourseId.Value
                   };

                   // add modules Save old and new modulid
                   await _context.Modules.AddAsync(tmp2);
                   await _context.SaveChangesAsync();


                   cloneModules.Add(new CloneModuleModel
                   {
                       Id = mod.Id,
                       Name = mod.Name,
                       Description = mod.Description,
                       StartDate = mod.StartDate.AddDays(noOfDays),
                       EndDate = mod.EndDate.AddDays(noOfDays),
                       CourseId = CloneFormModel.NewCourseId.Value,
                       NewModuleid=tmp2.Id

                   });
                   foreach(var act in coursedata.Modules.Where(m=> m.Id==mod.Id).Select(m => m.LMSActivities).FirstOrDefault())
                   {
                       LMSActivity tmpact = new LMSActivity
                       {
                           Name = act.Name
                       ,
                           Description = act.Description
                       ,
                           StartDate = act.StartDate.AddDays(noOfDays)
                       ,
                           EndDate = act.EndDate.AddDays(noOfDays)
                       ,
                           ActivityTypeId = act.ActivityTypeId
                       ,
                           ModuleId = tmp2.Id
                       };
                       // add activitities save old and new activitiyid
                       await _context.LMSActivity.AddAsync(tmpact);
                       await _context.SaveChangesAsync();

                       cloneActivities.Add(new CloneActivityModel
                       {
                           Id=act.Id,
                           Name=act.Name
    ,
                           StartDate = act.StartDate.AddDays(noOfDays)
                     ,
                           EndDate = act.EndDate.AddDays(noOfDays)
                           ,
                           ActivityTypeId = act.ActivityTypeId
                           ,moduleid = tmp2.Id
                           ,NewActivityId= tmpact.Id

                       });
                   }

               }


               // get documents
               List<Document> documents = new List<Document>();
               var Codoc = await _context.Documents.Where(d => d.CourseId.ToString() == CloneFormModel.Id).Where(d => (tempteacher.Select(u => u.Id).Contains(d.LMSUserId)))
                       .Select(d => new Document
                       {
                           Name = d.Name,
                           UploadDate = DateTime.Now,
                           Description = d.Description,
                           Path = d.Path,
                           LMSUserId = userid,
                           CourseId = CloneFormModel.NewCourseId.Value,
                           ModuleId = null,
                           LMSActivityId = null,
                           DocumentTypeId = d.DocumentTypeId
                       }
                ).ToArrayAsync();


               documents.AddRange(Codoc);
                   //


             var Modoc=  _context.Documents.Where(d =>  (tempteacher.Select(u => u.Id).Contains(d.LMSUserId)))
                   .Join(cloneModules,
                       d => d.ModuleId,
                      (CloneModuleModel m) => m.Id,
                       (d, m) => 
                         new Document
                         { 
                             Name = d.Name,
                             UploadDate = DateTime.Now,
                             Description = d.Description,
                             Path = d.Path,
                             LMSUserId = userid,
                             CourseId = null,
                             ModuleId = m.NewModuleid,
                             LMSActivityId = null,
                             DocumentTypeId = d.DocumentTypeId
                         }

                   );
               documents.AddRange(Modoc);

               var Actdoc = _context.Documents.Where(d => (tempteacher.Select(u => u.Id).Contains(d.LMSUserId)))
                  .Join(cloneActivities,
                      d => d.LMSActivityId,
                     (CloneActivityModel m) => m.Id,
                      (d, m) => new Document
                      {
                          Name = d.Name,
                          UploadDate = DateTime.Now,
                          Description = d.Description,
                          Path = d.Path,
                          LMSUserId = userid,
                          CourseId = null,
                          ModuleId = null,
                          LMSActivityId = m.NewActivityId,
                          DocumentTypeId = d.DocumentTypeId
                      }
                  );
               documents.AddRange(Actdoc);
               // update documents, courseid, moduleid and activitiyid
              // documents.Where(d => d.CourseId != null && d.CourseId.ToString() == CloneFormModel.Id).ToList().ForEach(d => d.CourseId = CloneFormModel.NewCourseId);

               // add documents
               await _context.Documents.AddRangeAsync(documents.Select(d => new Document
               {
                   Name = d.Name,
                   UploadDate = DateTime.Now,
                   Description = d.Description,
                   Path = d.Path,
                   LMSUserId =userid,
                   CourseId=d.CourseId,
                   ModuleId=d.ModuleId,
                   LMSActivityId=d.LMSActivityId,
                   DocumentTypeId=d.DocumentTypeId
                 }));
               await _context.SaveChangesAsync();


               // if a img add img.
               if (CloneFormModel.FileData!= null && CloneFormModel.FileData.Length > 0)
               {
                   string path = _programrepository.GetCourseImageUploadPath();
                   _documentrepository.UploadFile(CloneFormModel.FileData, path);
               }





               return Ok(new Course());*/
        }
        // PUT: api/Courses1
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> PutCourse(string id, [FromForm] CourseViewModel editModel)
        {
            //if (editModel.criD==null)
            if(id != editModel.criD)
            {
                return BadRequest();
            }

            Guid Crid = new Guid(editModel.criD);

            Course edCourse = new Course {
                Id = Crid,
                Name = editModel.Name,
                StartDate = editModel.StartDate,
                Description = editModel.Description,
                CourseImgPath = @"..\assets\img\" + editModel.FileData.FileName
            };

            _context.Entry(edCourse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(Crid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //Upload...But skip the empty one from edit course.
            if (editModel.FileData.Length>0)
            {
                string path = _programrepository.GetCourseImageUploadPath();
                _documentrepository.UploadFile(editModel.FileData, path);
            }

            return NoContent();
        }
 
        // POST: api/Courses1
        [HttpPost("Create"), DisableRequestSizeLimit]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<Course>> PostCourse([FromForm] CourseViewModel courseVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Course course = new Course
            {
                Name = courseVm.Name,
                StartDate = courseVm.StartDate,
                Description = courseVm.Description,
                CourseImgPath = @"..\assets\img\"+ courseVm.FileData.FileName
            };
         
           _context.Courses.Add(course);
            await _context.SaveChangesAsync();
          string path=  _programrepository.GetCourseImageUploadPath();
            _documentrepository.UploadFile(courseVm.FileData, path);
            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }
        // DELETE: api/Courses1/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<bool>> DeleteCourse(Guid iD)
        {
            var status= await _programrepository.RemoveCourseHelperAsync(iD);
            if (status)
            {
                var course = await _context.Courses.FindAsync(iD);
                if (course == null)
                {
                    return NotFound();
                }
                //Delete course. Data related in Modules and LMSActivity also are deleted.
                _context.Courses.Remove(course);

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {

                    _logger.LogDebug("Error deleting", ex.Message);
                }

                _logger.LogDebug("!!! Course of {name} deleted.", course.Name);


                return Ok(true);      //Send back 200.
            }
            else
                return StatusCode(500);
        }

        private bool CourseExists(Guid id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
