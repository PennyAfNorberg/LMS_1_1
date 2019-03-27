using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS_1_1.Data;
using LMS_1_1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using LMS_1_1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace LMS_1_1.Repository
{
    public class ProgramRepository : IProgramRepository

    {
        private readonly ApplicationDbContext _ctx;
        private readonly ILogger<ProgramRepository> _logger;
        private readonly UserManager<LMSUser> _userManager;
        private readonly IHostingEnvironment _environment;
        private readonly IDocumentRepository _DocumentRepository;

        //private readonly RoleManager<LMSUser> _roleManager;

        public ProgramRepository(ApplicationDbContext ctx, ILogger<ProgramRepository> logger, UserManager<LMSUser> userManager
        , IHostingEnvironment environment, IDocumentRepository DocumentRepository
            )
        {
            _ctx = ctx;
            _logger = logger;
            _userManager = userManager;
            _environment = environment;
            _DocumentRepository = DocumentRepository;
            // _roleManager = roleManager;
        }
        #region Commen
        public async Task AddEntityAsync(object model)
        {
            if (model is IProgram || model is ActivityType)
            {
                await _ctx.AddAsync(model);
            }
        }

        public void UpdateEntity(object model)
        {
            if (model is IProgram || model is ActivityType)
            {
                _ctx.Update(model);
            }
        }
        public void RemoveEntity(object model)
        {
            if (model is IProgram || model is ActivityType)
            {
                _ctx.Remove(model);
            }
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _ctx.SaveChangesAsync() > 0;
        }

        public string GetCourseImageUploadPath()
        {
            string rootPath = _environment.ContentRootPath;
            var folderName = Path.Combine("clientApp", "assets/img");
            var path = Path.Combine(rootPath, folderName);
            Directory.CreateDirectory(path);
            return path;
        }
        #endregion
        #region Course
        public async Task<IEnumerable<Course>> GetAllCoursesAsync(bool includeModule)
        {
            var courses = _ctx.Courses;
            if (includeModule)
            {
                return await courses
                           .Include(c => c.Modules).ToListAsync();
            }
            return await courses.ToListAsync();
        }




        public async Task<Course> GetCourseByIdAsync(Guid courseId, bool includeModule)
        {
            var course = _ctx.Courses
               .Where(c => c.Id == courseId);

            if (includeModule)
            {
                var res = await course
                          .Include(c => c.Modules)
                          .ThenInclude(m => m.LMSActivities)
                          .ThenInclude(a => a.ActivityType)
                           .FirstOrDefaultAsync();
                return res;
            }
            return await course
                   .FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Course>> GetCoursesForUserAsync(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if ((await _userManager.GetRolesAsync(user)).Any(r => r == "Teacher"))
            {
                var courses = _ctx.Courses;
                return await courses.ToListAsync();
            }
            else
            {
                var courseids = await _ctx.CourseUsers.Where(cu => cu.LMSUserId == userid).Select(cu => cu.CourseId).ToListAsync();
                var res = _ctx.Courses
                        .Where(c => courseids.Contains(c.Id));

                return await res.ToListAsync();

            }


        }

        public async Task<bool> RemoveCourseHelperAsync(Guid coursedid)
        {

            var docCourse = await _DocumentRepository.GetAllDocumentsForCourseAsync(coursedid);
            return await _DocumentRepository.RemoveDocumentRangeAsync(docCourse.ToList());
        }

        public async Task<Course> CloneCourseAsync(CloneFormModel cloneFormModel, string userid)
        {
            var tempteacher = await _userManager.GetUsersInRoleAsync("Teacher");
            var coursedata = await _ctx.Courses
                         .Include(c => c.Modules)
                         .ThenInclude(m => m.LMSActivities)
                         .ThenInclude(a => a.ActivityType)
                         .FirstOrDefaultAsync(c => cloneFormModel.Id == c.Id.ToString());

            var Alldocs = await _DocumentRepository.GetAllDocumentsForCourseAsync(Guid.Parse(cloneFormModel.Id));
            var Alldocclones = Alldocs.Where(d => (tempteacher.Select(u => u.Id).Contains(d.LMSUserId))).Select(d => new CloneDocumentModel
            {
                Name = d.Name,
                UploadDate = DateTime.Now,
                Description = d.Description,
                Path = d.Path,
                LMSUserId = userid,
                CourseId = d.CourseId,
                NewCourseId = null,
                ModuleId = d.ModuleId,
                NewModuleId = null,
                LMSActivityId = d.LMSActivityId,
                NewLMSActivityId = null,
                DocumentTypeId = d.DocumentTypeId
            }).ToList();

            DateTime dt = DateTime.Parse(cloneFormModel.NewDate.ToShortDateString());
            DateTime dt1 = DateTime.Parse(coursedata.StartDate.ToShortDateString());

            int noOfDays = (int)dt.Subtract(dt1).TotalDays; // dates to add
    
            // fix skip weekends
            // But not here, since it's choosed
            var tmpcourse = new Course
            {

                Name = cloneFormModel.Name,
                Description = cloneFormModel.Description,
                StartDate = cloneFormModel.NewDate,
                CourseImgPath = (cloneFormModel.FileData == null) ? coursedata.CourseImgPath : @"..\assets\img\" + cloneFormModel.FileData.FileName

            };
            await _ctx.Courses.AddRangeAsync(tmpcourse);
            _ctx.SaveChanges();
            cloneFormModel.NewCourseId = tmpcourse.Id;

            for (int i = 0; i < Alldocclones.Count(); i++)
            {
                if (Alldocclones[i].CourseId == Guid.Parse(cloneFormModel.Id))
                {
                    Alldocclones[i].NewCourseId = cloneFormModel.NewCourseId;
                }
            }
            DateTime tempstart, tempend;

            foreach (var mod in coursedata.Modules.Where(m => m.CourseId == Guid.Parse(cloneFormModel.Id)))
            {

                tempstart = mod.StartDate.AddDays(noOfDays);
                tempend = mod.EndDate.AddDays(noOfDays);
                if(cloneFormModel.CloneTypeId==1)
                {
                    tempstart = skipWeekEnd(tempstart);
                    tempend = skipWeekEnd(tempend);
                }

                Module tmp2 = new Module
                {
                    Name = mod.Name,
                    Description = mod.Description,
                    StartDate = tempstart,
                    EndDate = tempend,
                    CourseId = cloneFormModel.NewCourseId.Value
                };

                // add modules Save old and new modulid
                await _ctx.Modules.AddAsync(tmp2);
                _ctx.SaveChanges();
                for (int i = 0; i < Alldocclones.Count(); i++)
                {
                    if (Alldocclones[i].ModuleId == mod.Id)
                    {
                        Alldocclones[i].NewModuleId = tmp2.Id;
                    }
                }
                /*foreach (var doc in Alldocclones.Where(d => d.ModuleId == mod.Id))
                {
                    doc.NewModuleId = tmp2.Id;
                }*/
                //  foreach (var act in _ctx.LMSActivity.Where(m => m.ModuleId == mod.Id))
                foreach (var act in coursedata.Modules.Where(m => m.Id == mod.Id).Select(m => m.LMSActivities).FirstOrDefault())
                {
                    tempstart = act.StartDate.AddDays(noOfDays);
                    tempend = act.EndDate.AddDays(noOfDays);
                    if (cloneFormModel.CloneTypeId == 1)
                    {
                        tempstart = skipWeekEnd(tempstart);
                        tempend = skipWeekEnd(tempend);
                    }


                    LMSActivity tmpact = new LMSActivity
                    {
                        Name = act.Name,
                        Description = act.Description,
                        StartDate = tempstart,
                        EndDate = tempend,
                        ActivityTypeId = act.ActivityTypeId,
                        ModuleId = tmp2.Id
                    };
                    await _ctx.LMSActivity.AddAsync(tmpact);
                    await _ctx.SaveChangesAsync();
                    for (int i = 0; i < Alldocclones.Count(); i++)
                    {
                        if (Alldocclones[i].LMSActivityId == act.Id)
                        {
                            Alldocclones[i].NewLMSActivityId = tmpact.Id;
                        }
                    }
                    /* foreach (var doc in Alldocclones.Where(d => d.LMSActivityId == act.Id))
                     {
                         doc.NewLMSActivityId = tmpact.Id;
                     }*/
                }
            }
            // in with docs
            await _ctx.Documents.AddRangeAsync(Alldocclones.Select(d => new Document
            {
                Name = d.Name,
                UploadDate = DateTime.Now,
                Description = d.Description,
                Path = d.Path,
                LMSUserId = userid,
                CourseId = d.NewCourseId,
                ModuleId = d.NewModuleId,
                LMSActivityId = d.NewLMSActivityId,
                DocumentTypeId = d.DocumentTypeId
            }));
            await _ctx.SaveChangesAsync();


            // if a img add img.
            if (cloneFormModel.FileData != null && cloneFormModel.FileData.Length > 0)
            {
                string path = GetCourseImageUploadPath();
                _DocumentRepository.UploadFile(cloneFormModel.FileData, path);
            }





            return new Course();
        }

        private DateTime skipWeekEnd(DateTime start)
        {
            if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
            {
                return start.AddDays(2);
            }
            return start;
        }

        public async Task<bool> CourseExistsAsync(Guid courseId)
        {
            return await _ctx.Courses.AnyAsync(e => e.Id == courseId);
        }
        #endregion
        #region Module
        public async Task<IEnumerable<Module>> GetAllModulesAsync(bool includeActivities)
        {
            var modules = _ctx.Modules
                          .Include(c => c.Courses);

            if (includeActivities)
            {
                return await modules
                           .Include(m => m.LMSActivities)
                           .ToListAsync();
            }
            return await modules
                       .ToListAsync();

        }

        public async Task<Module> GetModuleByIdAsync(Guid moduleId, bool includeActivity)
        {
            var module = _ctx.Modules
                         .Include(c => c.Courses)
                         .Where(m => m.Id == moduleId);
            if (includeActivity)
            {
                return await module
                           .Include(m => m.LMSActivities)
                           .ThenInclude(a => a.ActivityType)
                           .FirstOrDefaultAsync();

            }
            else
            {
                return await module
                           .FirstOrDefaultAsync();
            }
        }

        public async Task<bool> RemoveModuleHelperAsync(Guid moduleid)
        {
            var docModule = await _DocumentRepository.GetAllDocumentsForModuleAsync(moduleid);
            return await _DocumentRepository.RemoveDocumentRangeAsync(docModule.ToList());
        }

        public async Task<bool> MoveModule(ModuleViewModel modelVm)
        {
            var old_module = await _ctx.Modules.Where(m => m.Id.ToString() == modelVm.Id).FirstOrDefaultAsync();
            var diffstart = old_module.StartDate - modelVm.StartDate;
            var diffend = old_module.EndDate - modelVm.EndDate;
            // Get old module
            // set Datediffs

            // update the module

            // get all later modules <-- func
            // update dates

            // call MoveActivity but as
            // no insert
            // given diffs

            return true;

        }


        public async Task<bool> ModuleExistsAsync(Guid moduleId)
        {
            return await _ctx.Modules.AnyAsync(e => e.Id == moduleId);
        }
        #endregion
        #region Activity
        public async Task<IEnumerable<LMSActivity>> GetAllActivitiesAsync()
        {
            return await _ctx.LMSActivity
                     .Include(a => a.ActivityType)
                      .Include(a => a.Modules).ToListAsync();
        }

        public async Task<LMSActivity> GetActivityByIdAsync(Guid activityId)
        {
            return await _ctx.LMSActivity
                 .Include(a => a.ActivityType)
                 .Include(a => a.Modules)
                 .FirstOrDefaultAsync(a => a.Id == activityId);

        }

        public async Task<bool> RemoveActivityHelperAsync(Guid activityid)
        {
            var docActivity = await _DocumentRepository.GetAllDocumentsForActivityAsync(activityid);
            return await _DocumentRepository.RemoveDocumentRangeAsync(docActivity.ToList());
        }

        public struct YearAndDate
        {
            public int Year { get; set; }
            public int Week { get; set; }
        }

         private YearAndDate GetWeeknrAndYear(DateTime d)
        {
            var YearAndDate = new YearAndDate();
            CultureInfo cul = CultureInfo.CurrentCulture;

            var firstDayWeek = cul.Calendar.GetWeekOfYear(
                d,
                CalendarWeekRule.FirstDay,
                DayOfWeek.Monday);

            YearAndDate.Week = cul.Calendar.GetWeekOfYear(
                d,
                CalendarWeekRule.FirstDay,
                DayOfWeek.Monday);
            YearAndDate.Year = d.Year;

            return YearAndDate;
        }

        private int DiffWeekends(YearAndDate yearAndDate1, YearAndDate yearAndDate2)
        {
            return (yearAndDate1.Year - yearAndDate2.Year) * 52 + yearAndDate1.Week - yearAndDate2.Week;
        }

        public async Task<bool> MoveLMSActivity(ActivityFormModel modelVm)
        {
            // Get old LMSActivity
            
            var old_Activity = await _ctx.LMSActivity
                .Where(m => m.Id == modelVm.Id.Value)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            // set Datediffs
            var diffstart = modelVm.StartDate - old_Activity.StartDate;
            var diffend = modelVm.EndDate - old_Activity.EndDate;


            int diffweeks = DiffWeekends(GetWeeknrAndYear(modelVm.EndDate), GetWeeknrAndYear(old_Activity.EndDate));
            LMSActivity ModActivity = new LMSActivity
            {
                Id = modelVm.Id.Value,
                Name = modelVm.Name,
                StartDate = modelVm.StartDate,
                EndDate = modelVm.EndDate,
                Description = modelVm.Description,
                ActivityTypeId = modelVm.ActivityTypeId,
                ModuleId = Guid.Parse(modelVm.moduleid)
            };

            _ctx.Entry(ModActivity).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(ModActivity.Id))
                {
                    return false ;
                }
                else
                {
                    throw;
                }
            }

            if (diffend != TimeSpan.Zero)
               return await SetLaterActivitesAsync(modelVm, diffstart, diffend, diffweeks);
           
            // Set Later LMSActivities  <-- func
            // incl modules
            return true;
        }



        private async Task<bool> SetLaterActivitesAsync(ActivityFormModel modelVm, TimeSpan diffstart, TimeSpan diffend,int diffweeks)
        {
            var currmodulid = Guid.Parse(modelVm.moduleid);
            // first M
            var firstM = await _ctx.Modules
                .Where(m => m.Id == currmodulid)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            var courseid = firstM.CourseId;
            DateTime Workstart = firstM.StartDate;
            DateTime WorkEnd = firstM.EndDate + diffend;
            int diffweeksres= DiffWeekends(GetWeeknrAndYear(WorkEnd), GetWeeknrAndYear(firstM.EndDate))- diffweeks;
            if(WorkEnd.DayOfWeek==DayOfWeek.Saturday || WorkEnd.DayOfWeek==DayOfWeek.Sunday)
            {
                diffweeksres++;
            }
            WorkEnd += TimeSpan.FromDays(2*diffweeksres)  ;
            // update first M, just E
            Module module = new Module
            {
                Id = firstM.Id,
                Name = firstM.Name,
                StartDate = Workstart,
                EndDate = WorkEnd,
                Description = firstM.Description,
                CourseId = firstM.CourseId
            };

            var status= await UpdateModule(module);

            
            if(status)
            {
                // Then All Later A in firstM , start and end
                var activites = await _ctx.LMSActivity
                    .Where(a => a.ModuleId == firstM.Id && a.Id != modelVm.Id && a.EndDate >= modelVm.EndDate.Add(diffend))
                    .AsNoTracking()
                    .ToListAsync(); 
                foreach(var activity in activites)
                {
                    if (!status) break;
                    GetStartAndEnd(diffend, out Workstart, out WorkEnd, diffweeks, activity.StartDate, activity.EndDate);

                    LMSActivity modActivity = new LMSActivity
                    {
                        Id = activity.Id,
                        Name = activity.Name,
                        StartDate = Workstart,
                        EndDate = WorkEnd,
                        Description = activity.Description,
                        ModuleId = activity.ModuleId,
                        ActivityTypeId = activity.ActivityTypeId
                    };
                    status = await UpdateActivity(modActivity);

                }

            }

            if (status)
            { // then foreach later M
                var modules = await _ctx.Modules
                    .Where(m => m.CourseId == courseid && m.Id != module.Id && m.EndDate >= module.EndDate.Add(diffend))
                    .AsNoTracking()
                    .ToListAsync();
                foreach (var modul in modules)
                {
                    // update M start & end
                    if (!status) break;

                    GetStartAndEnd(diffend, out Workstart, out WorkEnd,  diffweeks, modul.StartDate, modul.EndDate);
                    Module edmodule = new Module
                    {
                        Id = modul.Id,
                        Name = modul.Name,
                        StartDate = Workstart,
                        EndDate = WorkEnd,
                        Description = modul.Description,
                        CourseId = modul.CourseId
                    };

                    status = await UpdateModule(edmodule);
                    if(status)
                    {
                        var edActivites = await _ctx.LMSActivity
                            .Where(a => a.ModuleId == edmodule.Id && a.Id != modelVm.Id && a.EndDate >= module.EndDate.Add(diffend))
                            .AsNoTracking()
                            .ToListAsync();
                        foreach (var activity in edActivites)
                        {
                            if (!status) break;
                            GetStartAndEnd(diffend, out Workstart, out WorkEnd,  diffweeks, activity.StartDate, activity.EndDate);
                            LMSActivity modActivity = new LMSActivity
                            {
                                Id = activity.Id,
                                Name = activity.Name,
                                StartDate = Workstart,
                                EndDate = WorkEnd,
                                Description = activity.Description,
                                ModuleId = activity.ModuleId,
                                ActivityTypeId = activity.ActivityTypeId
                            };
                            status = await UpdateActivity(modActivity);

                        }
                    }
                }

            }
                
                
                // update all A start & end

                return status;

        }

        private void GetStartAndEnd(TimeSpan diffend, out DateTime Workstart, out DateTime WorkEnd, int diffweeksres, DateTime startDate, DateTime endDate)
        {
            
            Workstart = startDate + diffend;
            WorkEnd = endDate + diffend;
            int diffweeksresend = DiffWeekends(GetWeeknrAndYear(WorkEnd), GetWeeknrAndYear(endDate))- diffweeksres;
            int diffweeksresstart = DiffWeekends(GetWeeknrAndYear(Workstart), GetWeeknrAndYear(startDate)) - diffweeksres;
            if(Workstart.DayOfWeek==DayOfWeek.Saturday || Workstart.DayOfWeek==DayOfWeek.Sunday)
            {
                diffweeksresstart++;
            }
            if (WorkEnd.DayOfWeek == DayOfWeek.Saturday || WorkEnd.DayOfWeek == DayOfWeek.Sunday)
            {
                diffweeksresend++;
            }
            Workstart += TimeSpan.FromDays(2 * diffweeksresstart);
            WorkEnd += TimeSpan.FromDays(2 * diffweeksresend);
        }

        private async Task<bool> UpdateActivity(LMSActivity modActivity)
        {
            _ctx.Entry(modActivity).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(modActivity.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        private async Task<bool> UpdateModule(Module module)
        {
            _ctx.Entry(module).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(module.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        private bool ModuleExists(Guid id)
        {
            return _ctx.Modules.Any(e => e.Id == id);
        }

        private bool ActivityExists(Guid id)
        {
            return _ctx.LMSActivity.Any(e => e.Id == id);
        }

        public async Task<bool> LMSActivityExistsAsync (Guid activityId)
        {
            return await _ctx.LMSActivity.AnyAsync(e => e.Id == activityId);
        }

        #endregion
        #region ActivityType
        public async Task<IEnumerable<ActivityType>> GetAllActivityTypesAsync ()
        {
            return await _ctx.ActivityTypes.ToListAsync();
        }

        public async Task<ActivityType> GetAllActivityTypesByIdAsync (int activityTypeId)
        {
            return await _ctx.ActivityTypes
                .FirstOrDefaultAsync(a => a.Id == activityTypeId);
        }

        public async Task<bool> ActivityTypeExistsAsync (int activityTypeId)
        {
            return await _ctx.ActivityTypes.AnyAsync(e => e.Id == activityTypeId);
        }
        #endregion

        #region Token user

        public async Task AddTokenUser(string token, string userid)
        {
            var model = new TokenUser
            {
                Token = token,
                LMSUserId = userid
            };
            
            await _ctx.AddAsync(model);
        }

        public async Task<bool> RemoveTokenUser(string token)
        {
            var models = _ctx.TokenUsers.Where(tu => tu.Token == token);

            _ctx.RemoveRange(models);
            return await _ctx.SaveChangesAsync() > 0;
        }


        public async Task<bool> IsTeacher(string token)
        {
            var User = _ctx.TokenUsers
                 .Include(tu => tu.LMSUser)
                 .FirstOrDefault(tu => tu.Token == token)
                 ?.LMSUser;
           var roles= await _userManager.GetRolesAsync(User);

            return roles.Any(r => r == "Teacher");


        }




        #endregion

        public async Task<bool> CheckIfModuleInRange(string courseid, DateTime start, DateTime end)
        {
          return await  _ctx.Modules
                 .Where(m => m.CourseId.ToString() == courseid)
                 .Where(u => ((u.StartDate <= start && u.EndDate >= start)
                     || (u.StartDate <= end && u.EndDate >= end))
             ).AnyAsync();
        }

        public async Task<bool> CheckIfActivityInRange(string modulid, DateTime start, DateTime end)
        {
            return await _ctx.LMSActivity
         .Where(m => m.ModuleId.ToString() == modulid)
         .Where(u => ((u.StartDate <= start && u.EndDate >= start)
             || (u.StartDate <= end && u.EndDate >= end))
     ).AnyAsync();
        }


    }
}