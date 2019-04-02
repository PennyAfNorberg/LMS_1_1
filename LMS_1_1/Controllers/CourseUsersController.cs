using LMS_1_1.Data;
using LMS_1_1.Models;
using LMS_1_1.Repository;
using LMS_1_1.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace LMS_1_1.Controllers
{
    // [Route("api/[controller]")]
    // [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CourseUsersController : Controller
    //ControllerBase
    //Controller

    //ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CoursesController> _logger;
        private readonly ICourseUsersRepository _repository;
        private readonly UserManager<LMSUser> _userManager;

        public CourseUsersController(ICourseUsersRepository repository, ILogger<CoursesController> logger, ApplicationDbContext context, UserManager<LMSUser> userManager)
        {
            _context = context;
            _logger = logger;
            _repository = repository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult<ICollection<CourseUserViewModel>>> GetusersOn([FromBody] CourseIdViewModel CourseId)
        {
            var res = (await _repository.GetUsersAsync(CourseId.CourseId, true))
                .Select(cu => new CourseUserViewModel { Userid = cu.Id, FirstName = cu.FirstName, LastName = cu.LastName }).ToList();

          

            return Ok(res);

        }

        [HttpPost]
        public async Task<ActionResult<string>> GetCourseName([FromBody] CourseIdViewModel CourseId)
        {
            var res = Json(new {
                            Name = (await _repository.GetCourseNameAsync(CourseId.CourseId))
                    });
            return Ok(res);

        }


        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<ICollection<CourseUserViewModel>>> GetusersOff([FromBody] CourseIdViewModel CourseId)
        {
            var res = (await _repository.GetUsersAsync(CourseId.CourseId, false))
                .Select(cu => new CourseUserViewModel { Userid = cu.Id, FirstName = cu.FirstName, LastName = cu.LastName }).ToList();
            return Ok(res);

        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<ICollection<Course>>> GetCoursesOff([FromBody] UserIdViewModel UserId)
        {

            var res =await  _repository.GetCoursesOffAsync(UserId.UserId);

            /* var res = (await _repository.GetUsers(CourseId.CourseId, false))
                 .Select(cu => new CourseUserViewModel { Userid = cu.Id, FirstName = cu.FirstName, LastName = cu.LastName }).ToList();*/
            return Ok(res);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<ICollection<Course>>> GetCoursesOn([FromBody] UserIdViewModel UserId)
        {

            var res = await _repository.GetCoursesOnAsync(UserId.UserId);

            /* var res = (await _repository.GetUsers(CourseId.CourseId, false))
                 .Select(cu => new CourseUserViewModel { Userid = cu.Id, FirstName = cu.FirstName, LastName = cu.LastName }).ToList();*/
            return Ok(res);
        }


        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<string>> GetUserName([FromBody] UserIdViewModel UserId)
        {

            var workonuser = await _userManager.FindByIdAsync(UserId.UserId);

            var res = Json(new
            {
                Name = workonuser.FirstName +" "+workonuser.LastName

             
        });

            /* var res = (await _repository.GetUsers(CourseId.CourseId, false))
                 .Select(cu => new CourseUserViewModel { Userid = cu.Id, FirstName = cu.FirstName, LastName = cu.LastName }).ToList();*/
            return Ok(res);
        }
        [HttpPost]
        public async Task<ActionResult<string>> GetUser([FromBody] CourseIdViewModel userid)
        {
            var id = userid.CourseId;
            if (!User.IsInRole("Teacher"))
            {
                 if(id != null)
                    throw new AccessViolationException("A student may just access him/her-self");
               
            }
            var usertmp = await _userManager.FindByNameAsync(User.Identity.Name);
            if (id == null)
            {
                id = usertmp.Id;
            }

            List<LMSUser> workonuser = await _userManager.Users.Where(u => id==null || u.Id==id)
                
                .ToListAsync();
            /* var res = workonuser.Select(
                    u => new ManageUserViewModel
                   {
                         Id = u.Id,
                         Email = u.Email,
                         FirstName = u.FirstName,
                         LastName = u.LastName,
                         Password = "",
                         Confirmpassword = "",

                     }


                 );*/
            var res = new List<ManageUserViewModel>();

            foreach(var user in workonuser)
            {
                var roles =await _userManager.GetRolesAsync(user);
                if (roles.Count == 1)
                {
                    res.Add(
                            new ManageUserViewModel
                            {
                                Id = user.Id,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Password = "",
                                Confirmpassword = "",
                                Role = roles[0]
                            }
                        );
                }
                else
                {
                    res.Add(
                            new ManageUserViewModel
                            {
                                Id = user.Id,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Password = "",
                                Confirmpassword = "",
                                Role = string.Join(", ", roles)
                            }
                        );
                }

            }

         /*   var res = Json(new
            {
                Name = workonuser.FirstName + " " + workonuser.LastName


            });

            */

            /* var res = (await _repository.GetUsers(CourseId.CourseId, false))
                 .Select(cu => new CourseUserViewModel { Userid = cu.Id, FirstName = cu.FirstName, LastName = cu.LastName }).ToList();*/
            return Ok(res);
        }



        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<string>> GetUsers()
        {

            List<LMSUser> workonuser = await _userManager.Users

                .ToListAsync();
            /* var res = workonuser.Select(
                    u => new ManageUserViewModel
                   {
                         Id = u.Id,
                         Email = u.Email,
                         FirstName = u.FirstName,
                         LastName = u.LastName,
                         Password = "",
                         Confirmpassword = "",

                     }


                 );*/
            var res = new List<ManageUserViewModel>();

            foreach (var user in workonuser)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Count == 1)
                {
                    res.Add(
                            new ManageUserViewModel
                            {
                                Id = user.Id,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Password = "",
                                Confirmpassword = "",
                                Role = roles[0]
                            }
                        );
                }
                else
                {
                    res.Add(
                            new ManageUserViewModel
                            {
                                Id = user.Id,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Password = "",
                                Confirmpassword = "",
                                Role = string.Join(", ", roles)
                            }
                        );
                }

            }

            /*   var res = Json(new
               {
                   Name = workonuser.FirstName + " " + workonuser.LastName


               });

               */

            /* var res = (await _repository.GetUsers(CourseId.CourseId, false))
                 .Select(cu => new CourseUserViewModel { Userid = cu.Id, FirstName = cu.FirstName, LastName = cu.LastName }).ToList();*/
            return Ok(res);
        }


        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<bool>> AddCoursesToStudent([FromBody] SaveUsercourseListViewmode savecouseListViewmodel)
        {
            //  var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                await _repository.RemoveAllCourseUsersForUserAsync(savecouseListViewmodel.UserId);

                foreach (var courseid in savecouseListViewmodel.CourseIds)
                {
                    await _repository.AddCourseUserAsync( courseid, savecouseListViewmodel.UserId);
                }

                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

         /*   var res = Json(new
            {
                Name = "Ok"
            });
            */
            return Ok(true);
        }

        [HttpGet("{CourseId}")]
        [Authorize]
        public ActionResult<Guid> GetStart(Guid CourseId)
        {
           
            return Ok(CourseId);
        }

        // GET: api/CourseUsers/5
        [HttpGet("{CourseId}")]
        [Authorize]
        public async Task<ActionResult<ICollection<CourseUser>>> GetCourseUsers([FromBody] string CourseId)
        {
            return await _repository.GetCourseUsers(CourseId).ToListAsync();
        }

        // GET: api/CourseUsers/5
        [HttpGet]
        [Authorize]
        //public IQueryable<CourseUser> GetCoursesForUsers(string LMSUserId)
        public async Task<ActionResult<ICollection<CourseUser>>> GetCoursesForUsers()
        {
            var userid = _userManager.GetUserId(User);
            // to do Teacher case
            var courseUser = await _repository.GetCoursesForUsers(userid).ToListAsync();


            if (courseUser == null)
            {
                return NotFound();
            }

            return courseUser;
        }

        // PUT: api/CourseUsers/5
        [HttpPut("{Courseid}")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<IActionResult> PutCourseUser(Guid Courseid, CourseUser courseUser)
        {
            if (Courseid != courseUser.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(courseUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseUserExists(Courseid, courseUser.LMSUserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpPost]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<Boolean>> AddStudentsToCourse([FromBody] SaveUsercourseListViewmodel savecouseListViewmodel)
        {
            //  var user = await _userManager.FindByNameAsync(User.Identity.Name);
            // atm SavecouseListViewmodel.Courseid is UserId
            // and SavecouseListViewmodel.Userids is list of courseids
            try
            {
                await _repository.RemoveAllCourseUsersForCourseAsync(savecouseListViewmodel.Courseid);

                foreach (var userid in savecouseListViewmodel.Userids)
                {
                    await _repository.AddCourseUserAsync(  savecouseListViewmodel.Courseid, userid);
                }

                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

           /* var res = Json(new
            {
                Name = "Ok"
            });*/

            return Ok(true);
        }


        [HttpPost]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<string>> ToggleCourseUser(Guid CourseId, string UserId)
        {
            CourseUser courseUser = new CourseUser { CourseId = CourseId, LMSUserId = UserId };
            courseUser.Course = _context.Courses.FirstOrDefault(c => c.Id == courseUser.CourseId);
            courseUser.LMSUser = _context.LMSUsers.FirstOrDefault(u => u.Id == courseUser.LMSUserId);

            // if (_context.CourseUsers.Contains(courseUser))
            var tempcu = _context.CourseUsers.FirstOrDefault(cu => cu.LMSUserId == courseUser.LMSUserId && cu.CourseId == courseUser.CourseId);
            if (tempcu != null)
            {
                _context.CourseUsers.Remove(tempcu);
            }
            else
            {
                _context.CourseUsers.Add(courseUser);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourseUserExists(courseUser.CourseId, courseUser.LMSUserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return "ok";
        }

        // POST: api/CourseUsers
        [HttpPost]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<CourseUser>> PostCourseUser(CourseUser courseUser)
        {
            _context.CourseUsers.Add(courseUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourseUserExists(courseUser.CourseId, courseUser.LMSUserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCourseUser", new { Courseid = courseUser.CourseId }, courseUser);
        }

        // DELETE: api/CourseUsers/5
        [HttpDelete("{Courseid,LMSUserId}")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<CourseUser>> DeleteCourseUser(Guid Courseid, string LMSUserId)
        {
            var courseUser = await _context.CourseUsers.FirstOrDefaultAsync(cu => cu.CourseId == Courseid && cu.LMSUserId == LMSUserId);
            if (courseUser == null)
            {
                return NotFound();
            }

            _context.CourseUsers.Remove(courseUser);
            await _context.SaveChangesAsync();

            return courseUser;
        }

        private bool CourseUserExists(Guid Courseid, string LMSUserid)
        {
            return _context.CourseUsers.Any(e => e.CourseId == Courseid && e.LMSUserId == LMSUserid);
        }
    }
}
