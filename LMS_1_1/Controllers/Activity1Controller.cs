using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS_1_1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS_1_1.Data;
using LMS_1_1.ViewModels;
using LMS_1_1.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace LMS_1_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Activity1Controller : ControllerBase
    {
        private UserManager<LMSUser> _userManager;

        private IProgramRepository _programrepository;
        private IDocumentRepository _documentrepository;
        private ApplicationDbContext _context;
        private IHostingEnvironment _environment;

        public Activity1Controller(IProgramRepository programrepository
            , IDocumentRepository documentrepository
            , ILogger<CoursesController> logger
            , ApplicationDbContext context
            , IHostingEnvironment environment
            , UserManager<LMSUser> userManager)
        {
            _userManager = userManager;
            _programrepository = programrepository;
            _documentrepository = documentrepository;
            _context = context;
            _environment = environment;


        }

        // GET: api/Activity1/5
        [HttpGet("{id}")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<ActivityViewModel>> GetActivityById(string id)
        {
            Guid idG = Guid.Parse(id);
            string userid = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            ActivityViewModel Activity = await _programrepository.GetActivityByIdWithColorAsync(idG, userid);


            if (Activity == null)
            {
                return NotFound();
            }

            return Ok(Activity);
        }

        [HttpGet("ActivityTypes")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<ICollection<ActivityType>>> GetActivityTypes()
        {
          return await  _context.ActivityTypes.ToListAsync();
        }


        // POST: api/Activity1
        // POST: api/Module1
        [HttpPost("PostActivity")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        //public async Task<ActionResult<LMSActivity>> PostActivity([FromBody] dynamic activtyVm)
        public async Task<ActionResult<LMSActivity>> PostActivity([FromBody] ActivityFormModel activtyVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            LMSActivity activity = new LMSActivity
            {
                Name = activtyVm.Name,
                StartDate = activtyVm.StartDate,
                EndDate = activtyVm.EndDate,
                Description = activtyVm.Description,
                ModuleId = Guid.Parse(activtyVm.moduleid),
                ActivityTypeId= activtyVm.ActivityTypeId
            };

            _context.Add(activity);
            await _context.SaveChangesAsync();
            return Created("", activity);
        }

        // PUT: api/Activity1/5
        [HttpPut("Edit/{id}")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<LMSActivity>> Edit(string id, [FromBody] ActivityFormModel activtyVm)
        {
            //if (editModel.criD==null)
            if (id != activtyVm.Id.ToString())
            {
                return BadRequest();
            }

          //  Guid Crid = new Guid(activtyVm.id);

            LMSActivity Activity = new LMSActivity
            {
                Id = activtyVm.Id.Value,
                Name = activtyVm.Name,
                StartDate = activtyVm.StartDate,
                EndDate = activtyVm.EndDate,
                Description = activtyVm.Description,
                ActivityTypeId= activtyVm.ActivityTypeId,
                ModuleId=Guid.Parse(activtyVm.moduleid)
            };

            _context.Entry(Activity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(activtyVm.Id.Value))
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

        [HttpPut("Move/{id}")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<LMSActivity>> Move(string id, [FromBody]  ActivityFormModel activtyVm)
        {
            //if (editModel.criD==null)
            if (id != activtyVm.Id.ToString())
            {
                return BadRequest();
            }

            //  Guid Crid = new Guid(activtyVm.id);
            var status= await _programrepository.MoveLMSActivityAsync(activtyVm);

            if(status)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }


            
        }

        // DELETE: api/Activity1/5
        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Teacher")]
        //public async Task Delete(int id)
        //{
        //    var activity = _context.LMSActivity.FindAsync(id);
        //    if (activity == null)
        //    {
        //        return;
        //    }

        //    _context.Remove(activity);
        //    await _context.SaveChangesAsync();

        //}

        private bool ActivityExists(Guid id)
        {
            return _context.LMSActivity.Any(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<ActionResult<bool>> Delete(Guid iD)
        {
            var status = await _programrepository.RemoveActivityHelperAsync(iD);
            if (status)
            {
                var actv = await _context.LMSActivity.FindAsync(iD);
                if (actv == null)
                {
                    return NotFound();
                }
/*
                //delete documents associated to it.
             var acDocuments =await _documentrepository.GetDocumentsByIdOwnerAsync(iD);
                foreach (Document doc in acDocuments)
                {
                   await _documentrepository.RemoveDocumentAsync(doc);
                }*/
          

                _context.LMSActivity.Remove(actv);
                _context.SaveChanges();
                return Ok(true);      //Send back 200.
            }
            else
                return StatusCode(500);
        }
    }
}
