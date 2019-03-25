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
using LMS_1_1.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

namespace LMS_1_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Module1Controller : ControllerBase
    {
        private UserManager<LMSUser> _userManager;

        private IProgramRepository _programrepository;
        private IDocumentRepository _documentrepository;
        private ApplicationDbContext _context;
        private IHostingEnvironment _environment;

        public Module1Controller(IProgramRepository programrepository
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



        // GET: api/Module1/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<Module>> GetModuleById(string id)
        {
            Guid idG = Guid.Parse(id);
            Module module = await _context.Modules.FindAsync(idG);


            if (module == null)
            {
                return NotFound();
            }

            return Ok(module);
        }
        // POST: api/Module1
        [HttpPost("PostModule")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<Module>> PostModule([FromBody] ModuleViewModel modelVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Module module = new Module
            {
                Name = modelVm.Name,
                StartDate = modelVm.StartDate,
                EndDate = modelVm.EndDate,
                Description = modelVm.Description,
                CourseId = modelVm.CourseId
            };

            _context.Add(module);
            await _context.SaveChangesAsync();
            return Created("", module);
        }

        // PUT: api/Module1/5
        [HttpPut("{id}")]
        [AcceptVerbs("Edit")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<LMSActivity>>  Put(string id, [FromBody]  ModuleViewModel modelVm)
        {
            //if (editModel.criD==null)
            if (id != modelVm.Id.ToString())
            {
                return BadRequest();
            }
            //MoveModule(ModuleViewModel modelVm)
            _programrepository.MoveModule(modelVm);
            //  Guid Crid = new Guid(activtyVm.id);
            var diffstart = modelVm.StartDate;
            var diffend = modelVm.EndDate;
            Module module = new Module
            {
                Id = Guid.Parse(modelVm.Id),
                Name = modelVm.Name,
                StartDate = modelVm.StartDate,
                EndDate = modelVm.EndDate,
                Description = modelVm.Description,
                CourseId= modelVm.CourseId
            };

            _context.Entry(module).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(module.Id))
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

        // PUT: api/Module1/5
        [HttpPut("{id}")]
        [AcceptVerbs("Move")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<LMSActivity>> PutMove(string id, [FromBody]  ModuleViewModel modelVm)
        {
            //if (editModel.criD==null)
            if (id != modelVm.Id.ToString())
            {
                return BadRequest();
            }

            //  Guid Crid = new Guid(activtyVm.id);

            Module module = new Module
            {
                Id = Guid.Parse(modelVm.Id),
                Name = modelVm.Name,
                StartDate = modelVm.StartDate,
                EndDate = modelVm.EndDate,
                Description = modelVm.Description,
                CourseId = modelVm.CourseId
            };

            _context.Entry(module).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(module.Id))
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

        // DELETE: api/module1/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<bool>>  Delete(Guid iD)
        {
            var status = await _programrepository.RemoveModuleHelperAsync(iD);
            if (status)
            {
                try
                {


                    var module = await _context.Modules.FindAsync(iD);
                    if (module == null)
                    {
                        return NotFound();
                    }

                    _context.Modules.Remove(module);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return Ok(true);      //Send back 200.
            }
            else
                return StatusCode(500);
        }


        [HttpPost("TestIfInRange")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<bool>> TestIfInRange([FromBody] DubbParas parmas)
        {
            bool res = false;
            if (parmas.Dubbtype == "Module")
            {
                res = await _programrepository.CheckIfModuleInRange(parmas.Dubbid, parmas.Dubbstart, parmas.Dubbend);


            }
            else if (parmas.Dubbtype == "Activity")
            {
                res = await _programrepository.CheckIfActivityInRange(parmas.Dubbid, parmas.Dubbstart, parmas.Dubbend);
            }
            else
            {
                return StatusCode(500, "invalid type")
;
            }
            return Ok(res);
        }
        private bool ModuleExists(Guid id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }
    }
}
