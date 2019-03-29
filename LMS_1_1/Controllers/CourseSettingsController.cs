﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS_1_1.Data;
using LMS_1_1.Models;
using LMS_1_1.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LMS_1_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CourseSettingsController : ControllerBase
    {
        private readonly UserManager<LMSUser> _userManager;
        private readonly ILogger<CourseSettingsController> _logger;
        private readonly IProgramRepository _programrepository;

        public CourseSettingsController(IProgramRepository programrepository
            , ILogger<CourseSettingsController> logger
            , UserManager<LMSUser> userManager
            )
        {
            _programrepository = programrepository;
            _logger = logger;
            _userManager = userManager;

        }
        // GET: api/CourseSettings
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CourseSettings/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CourseSettings
        [HttpPost]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/CourseSettings/5
        [HttpPut("{id}")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public void Delete(int id)
        {
        }
    }
}
