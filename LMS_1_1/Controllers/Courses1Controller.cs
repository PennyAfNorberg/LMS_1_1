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
            Course course = await _programrepository.GetCourseByIdAsync(idG);



            if (course == null)
            {
                return NotFound();
            }

            return course;
        }
        
        [HttpGet("All")]
        public async Task<ActionResult<CourseAllViewModel>> GetCourseAll(string id)
        {
               Guid idG = Guid.Parse(id);
            var course = await _programrepository.GetCourseByIdAllAsync(idG);
            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);


        }

        [HttpGet("CAndM")]
        public async Task<ActionResult<CourseAllViewModel>> GetCourseAndModule(string id)
        {
               Guid idG = Guid.Parse(id);

            var course = await _programrepository.GetCourseAndModule(idG);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }
        
        [HttpGet("AfromMid")]
        public async Task<ActionResult<ICollection<ActivityViewModel>>> GetActivitiesFromModulid(string id)
        {
            Guid idG = Guid.Parse(id);
            var res = await _programrepository.GetActivitiesFromModulid(idG);


            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        
        [HttpGet("MAndAfromMid")]
        public async Task<ActionResult<ModelAllViewModel>> GetModulesAndActivitiesFromModulid(string id)
        {
              Guid idG = Guid.Parse(id);
            var Module1 = await _programrepository.GetModulesAndActivitiesFromModulid(idG);
            

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

        [HttpPost("ModulesWithColor")]
        public async Task<ActionResult<List<ScheduleViewModel>[]>> GetModulesWithColour([FromBody] ScheduleFormModel scheduleFormModel)
        {
            string userid = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var Entities = await _programrepository.GetModulesWithColour(scheduleFormModel, userid);
            if (Entities == null)
            {
                return NotFound();
            }

            return Ok(Entities);
        }
        [HttpPost("ActivitiesWithColor")]
        public async Task<ActionResult<List<ScheduleViewModel>[]>> GetActivitiesWithColour([FromBody] ScheduleFormModel scheduleFormModel)
        {
            string userid = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var Entities = await _programrepository.GetActivitiesWithColour(scheduleFormModel, userid);
            if (Entities == null)
            {
                return NotFound();
            }

            return Ok(Entities);
        }

        [HttpPost("Clone"), DisableRequestSizeLimit]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<Course>> Clone([FromForm] CloneFormModel CloneFormModel )
        {



             string userid=  (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
             var res= await _programrepository.CloneCourseAsync(CloneFormModel, userid);

            return Ok(res);

  
        }
        // PUT: api/Courses1
        [HttpPut("{id}")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
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
        [Authorize(Roles = ConstDefine.R_TEACHER)]
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
        [Authorize(Roles = ConstDefine.R_TEACHER)]
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
