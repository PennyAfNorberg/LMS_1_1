using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LMS_1_1.Data;
using LMS_1_1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS_1_1.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _ctx;
        private readonly ILogger<ProgramRepository> _logger;
        private readonly IHostingEnvironment _environment;

        public DocumentRepository (ApplicationDbContext ctx, ILogger<ProgramRepository> logger, IHostingEnvironment environment)
        {
            _ctx = ctx;
            _logger = logger;
            _environment = environment;
        }
        public async Task AddDocumentAsync (Document model)
        {
            await _ctx.AddAsync(model);

        }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync ()
        {
            return await _ctx.Documents.ToListAsync();

        }


        public async Task<Document> GetDocumentByIdAsync (Guid documentId)
        {
            return await _ctx.Documents
                       .FirstOrDefaultAsync(d => d.Id == documentId);

        }

        public async Task<bool> IsExistDocumentByPathAsync (string path)
        {
            var document= await _ctx.Documents
                       .Where(d => d.Path == path).ToListAsync();
            if(document.Count>0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Document>> GetDocumentsByIdOwnerAsync (Guid OwnerId)
        {
            
            try
            {
               var  documents = await _ctx.Documents
                         .Where(d => (d.CourseId == OwnerId || d.ModuleId == OwnerId || d.LMSActivityId == OwnerId)).ToListAsync();
                return documents;
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
            return null;

        }

        public async Task<DocumentType> GetDocumentTypeByIdAsync (int documentTypeId)
        {
            return await _ctx.DocumentTypes
                        .FirstOrDefaultAsync(dt => dt.Id== documentTypeId);
        }

        public async Task RemoveDocumentAsync (Document model)
        {
            string fileNameTobeDeleted = model.Path;
            _ctx.Remove(model); 
           await   SaveAllAsync();
            bool isExist = await IsExistDocumentByPathAsync(fileNameTobeDeleted);
            if (isExist)
            {
                string folderpath = GetDocumentUploadPath();

                RemoveFile(folderpath, model.Path);
            }
        }

        public async Task<bool> RemoveDocumentRangeAsync(List<Document> docCourse)
        {
            foreach (var doc in docCourse)
            {
                try
                {
                    await RemoveDocumentAsync(doc);
                }
                catch (Exception ex)
                {

                    Console.Write(ex.Message);
                    return false;
                }

            }
            return true;
        }

        public async Task<bool> SaveAllAsync ()
        {
            try
            {
                  await _ctx.SaveChangesAsync() ;
            }
            catch(Exception ex)
            {
              Console.Write( ex.Message);
                return false;
            }
            return true;
        }

        public string GetDocumentUploadPath()
        {
            string rootPath = _environment.ContentRootPath;
            var folderName = Path.Combine("Attachements", "Documents");
            var path = Path.Combine(rootPath, folderName);
            Directory.CreateDirectory(path);
            return path;
        }
        public string UploadFile (IFormFile file,string path)
        {
            try
            {
                if (file.Length > 0)
                {
                   
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(path, fileName);
                    var uniqueFullPath = GetUniqueFilePath(fullPath);


                        using (var stream = new FileStream(uniqueFullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    string filename = Path.GetFileName(uniqueFullPath); ;


                    return filename;

                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }


        public  string GetUniqueFilePath (string filepath)
        {
          

            if (File.Exists(filepath))
            {
                string folder = Path.GetDirectoryName(filepath);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);
                int number = 1;

                Match regex = Regex.Match(filepath, @"(.+) \((\d+)\)\.\w+");

                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    filepath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                }
                while (File.Exists(filepath));
            }

            return filepath;
        }

        public FileStream DownloadFile (string path,string fileName)
        {
          
            var file = Path.Combine(path, fileName);
            if(File.Exists(file))
            return new FileStream(file, FileMode.Open, FileAccess.Read);
            return null;
        }

        public bool RemoveFile (string path,string fileName)
        {
            var file = Path.Combine(path, fileName);
            if (File.Exists(file))
            {
                File.Delete(file);
                return true;
            }
            return false;
        }

        public async Task<ICollection<Document>> GetAllDocumentsForCourseAsync(Guid CourseId)
        {
            var doc= await _ctx.Documents.Where(d => d.CourseId == CourseId).ToListAsync();
            foreach (var module in _ctx.Modules.Where(m => m.CourseId== CourseId))
            {
                doc.AddRange(await GetAllDocumentsForModuleAsync(module.Id));
            }
            return doc;
        }

        public async Task<ICollection<Document>> GetAllDocumentsForModuleAsync(Guid ModulId)
        {
            var doc = await  _ctx.Documents.Where(d => d.ModuleId == ModulId).ToListAsync();
            foreach (var activity in _ctx.LMSActivity.Where(a => a.ModuleId== ModulId)   )
            {
                doc.AddRange(await GetAllDocumentsForActivityAsync(activity.Id));
            }

            return doc;
        }

        public async Task<ICollection<Document>> GetAllDocumentsForActivityAsync(Guid ActivitityId)
        {
            return await _ctx.Documents.Where(d => d.LMSActivityId == ActivitityId).ToListAsync();
        }
    }
}
