using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS_1_1.Data;
using LMS_1_1.Models;
using LMS_1_1.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using LMS_1_1.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace LMS_1_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Documents1Controller : ControllerBase
    {
   
       
        private readonly IDocumentRepository _repository;
        private readonly ILogger<Documents1Controller> _logger;
        private readonly UserManager<LMSUser> _userManager;

        public Documents1Controller(IDocumentRepository repository, ILogger<Documents1Controller> logger, ApplicationDbContext context, UserManager<LMSUser> userManager)
        {
            _repository = repository;
            _logger = logger;
            _userManager = userManager;

        }

        // GET: api/Documents1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetDocuments()
        {
            return Ok(await _repository.GetAllDocumentsAsync());
        }

        // GET: api/Documents1/5
      [HttpGet]
        public async Task<ActionResult<Document>> GetDocument(string id)
        {
            Guid idG = Guid.Parse(id);
            var document = await _repository.GetDocumentByIdAsync(idG);

            if (document == null)
            {
                return NotFound();
            }

            return Ok(document);
        }

        // GET: api/Documents1/5
        [HttpGet("ByOwner")]
        public async Task<ActionResult<IEnumerable<Document>>> GetDocumentByOwner(string id)
        {
            Guid idG = Guid.Parse(id);
           var document = await _repository.GetDocumentsByIdOwnerAsync(idG);

            if (document == null)
            {
                return NotFound();
            }

             return Ok(document);
        }

        

       
      

        // POST: api/Documents1
       
        [HttpPost, DisableRequestSizeLimit]
        public async Task<ActionResult<Document>> PostDocument([FromForm] UploadDocumentInfoViewModel documentVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string path = _repository.GetDocumentUploadPath();
           string fileName=  _repository.UploadFile(documentVm.FileData,path);
            if(fileName!=null)
                {
                Document document = new Document
                {
                    Name = documentVm.Name,
                    Description = documentVm.Description,
                    UploadDate = DateTime.Now,
                    Path = fileName,
                    LMSUserId =_userManager.GetUserId(User),
                    DocumentTypeId = documentVm.DocumentTypeId,

                    CourseId = documentVm.DocOwnerTypeId == (int)DocOwnerType.Course ? documentVm.DocOwnerId : null,
                    ModuleId = documentVm.DocOwnerTypeId == (int)DocOwnerType.Module ? documentVm.DocOwnerId : null,
                    LMSActivityId = documentVm.DocOwnerTypeId == (int)DocOwnerType.Activity ? documentVm.DocOwnerId : null
                };
                await _repository.AddDocumentAsync(document);

                await _repository.SaveAllAsync();

                return CreatedAtAction("GetDocument", new { id = document.Id }, document);
            }
            return BadRequest();
        }

        [HttpPost("DownloadFile"), DisableRequestSizeLimit]
        public FileStream DownloadFile (string fileName)
        {
            string folderpath = _repository.GetDocumentUploadPath();
            return  _repository.DownloadFile(folderpath, fileName);
        }

        // DELETE: api/Documents1/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Document>> DeleteDocument(string id)
        {
            Guid idG = Guid.Parse(id);
            var document = await _repository.GetDocumentByIdAsync(idG);
            if (document == null)
            {
                return NotFound();
            }

         
           await _repository.RemoveDocumentAsync(document);
          

            return Ok(document);
        }

    }
}
