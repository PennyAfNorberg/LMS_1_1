using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using LMS_1_1.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using LMS_1_1.Models;

namespace LMS_1_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ColorModuleController : ControllerBase
    {
        private readonly UserManager<LMSUser> _userManager;
        private readonly ILogger<ColorModuleController> _logger;
        private readonly IProgramRepository _programrepository;

        public ColorModuleController(IProgramRepository programrepository
             , ILogger<ColorModuleController> logger
            , UserManager<LMSUser> userManager
            )
        {
            _programrepository = programrepository;
            _logger = logger;
            _userManager = userManager;
        }
        // GET: api/ColorModule
        [HttpGet("why")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ColorModule/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ColorModule
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/ColorModule/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
