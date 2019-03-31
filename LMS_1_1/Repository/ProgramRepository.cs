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
            /*start for debug */
            /*      var data = new ScheduleFormModel
                  {
                      CourseId = "fd26f900-0d75-4ba9-fbdb-08d6b2b4aba9",
                      StartTime = DateTime.Parse("2019-03-25 00:00:01"),
                      EndTime = DateTime.Parse("2019-03-31 23:59:59")
                  };
                  string userid = "f5608d74-bc13-4325-b2bd-c0db27b69206";
                  GetActivitiesWithColour(data, userid).Wait();

            string courseId = "fd26f900-0d75-4ba9-fbdb-08d6b2b4aba9";
            DateTime startDate = DateTime.Parse("2019-03-25 00:00:01");
            DateTime  endDate = DateTime.Parse("2019-03-31 23:59:59");
            GetCourseSettingsAsync(courseId, startDate, endDate).Wait();
            */

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




        public async Task<Course> GetCourseByIdAsync(Guid courseId)
        {
            var course = _ctx.Courses
               .Where(c => c.Id == courseId);


            return await course
                   .FirstOrDefaultAsync();

        }

        public async Task<CourseAllViewModel> GetCourseByIdAllAsync(Guid courseId)
        {
            var course1 = await _ctx.Courses
                                      .Include(c => c.Modules)
                                      .ThenInclude(m => m.LMSActivities)
                                      .ThenInclude(a => a.ActivityType)
                                      .FirstOrDefaultAsync(c => c.Id == courseId);



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
                         Name2 = (Guid.NewGuid()).ToString(),
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



            return course;

        }
         public async Task<CourseAllViewModel> GetCourseAndModule(Guid courseId)
        {
            var course1 = await _ctx.Courses
                          .Include(c => c.Modules)
                          .FirstOrDefaultAsync(c => c.Id == courseId);



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
                         Name2 = (Guid.NewGuid()).ToString(),
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

            return course;
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
            // clone settings if any
            var css= _ctx.CourseSettings.Where(cs => cs.CourseId.Value.ToString() == cloneFormModel.Id && cs.Date == null).FirstOrDefault();
            if(css!=null)
            {
                var ncs = new CourseSettings
                {
                    CourseId = cloneFormModel.NewCourseId,
                    StartTime = css.StartTime,
                   // StartLunch = css.StartLunch,
                   // EndLunch = css.EndLunch,
                    EndTime = css.EndTime
                };
                await _ctx.CourseSettings.AddRangeAsync(ncs);
                _ctx.SaveChanges();
            }
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

        public async Task<List<CourseSettingsViewModel>> GetCourseSettingsAsync(string courseId, DateTime? startDate, DateTime? endDate)
        {
            IFormatProvider culture = new CultureInfo("sv-SE", true);
            List<CourseSettingsViewModel> svar = new List<CourseSettingsViewModel>();
            List<DateTime> DateToCheck = new List<DateTime>();
            for (DateTime i = startDate.Value; i < endDate.Value; i=i.AddDays(1))
            {
                if(i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                    DateToCheck.Add(i);
            }
            foreach (var DateCheck in DateToCheck)
            {
                svar.AddRange( _ctx.CourseSettings
                    .Where(cs => (cs.CourseId.ToString() ?? courseId) == courseId && (cs.Date ?? DateCheck).ToString().Substring(0,10) == DateCheck.ToString().Substring(0, 10))
                    .Select( cs => new CourseSettingsViewModel
                    {
                        Id=cs.Id,
                        CourseId=cs.CourseId,
                        Date=cs.Date,
                        StartTime= DateCheck.ToString("yyyy-MM-dd") +" "+ cs.StartTime, 
                       // StartLunch=cs.StartLunch,
                       // EndLunch=cs.EndLunch,
                        EndTime= DateCheck.ToString("yyyy-MM-dd") + " "  + cs.EndTime,
                        ForDate=DateCheck
                    })
                    );
            }
            return svar;
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

        public async Task<ModelAllViewModel> GetModulesAndActivitiesFromModulid(Guid moduleId)
        {
            var Module = await _ctx.Modules
                            .Include(m => m.LMSActivities)
                           .ThenInclude(a => a.ActivityType)
                          .Where(m => m.Id == moduleId)
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
                CourseId = Module.CourseId
            };
            return Module1;
        }

        public async Task<List<ScheduleViewModel>[]> GetModulesWithColour(ScheduleFormModel scheduleFormModel, string userid)
        {
            ColorModule  defaultcolor;

           var  colorfromthis = _ctx.ColorModule
                                .Where(
                                cm =>
                                (cm.LMSUserId ?? userid) == userid
                                && cm.ModuleId != null
                                );


   
                defaultcolor =await  _ctx.ColorModule
                                .Where(
                                cm =>
                                (cm.LMSUserId ?? userid) == userid
                                && cm.ModuleId == null
                                ).FirstOrDefaultAsync();


            var coursesettings = await GetCourseSettingsAsync(scheduleFormModel.CourseId, scheduleFormModel.StartTime, scheduleFormModel.EndTime);
            var minCsstart = coursesettings.Min(cs => cs.StartTime);
            var maxCsend = coursesettings.Max(cs => cs.EndTime);

            var workthis = await _ctx.Modules
                            .Where(m => m.CourseId.ToString() == scheduleFormModel.CourseId 
                                && ((m.EndDate >= scheduleFormModel.StartTime   ) 
                                && (m.StartDate <= scheduleFormModel.EndTime)))
                            .GroupJoin(
                            colorfromthis,
                            m => m.Id,
                            cm => cm.ModuleId,
                            (x, y) => new { mod = x, color = y }
                            )
                            .SelectMany(
                             x => x.color.DefaultIfEmpty(),
                      (x, y) => new ScheduleViewModel
                      {
                          Id = x.mod.Id.ToString(),
                          Color = y.Color,
                          Name = x.mod.Name,
                          Description = x.mod.Description,
                          StartTime = x.mod.StartDate,
                          EndTime = x.mod.EndDate,
                          DayOfWeek = x.mod.StartDate.DayOfWeek
                      }).ToListAsync();



            workthis =  workthis
                .Select
                (
                 m => new ScheduleViewModel
                 {
                     Id = m.Id,
                     Name = m.Name,
                     StartTime = (DateTime.Parse(minCsstart) > m.StartTime) ? DateTime.Parse(minCsstart) : m.StartTime,
                     EndTime = (DateTime.Parse(maxCsend) < m.EndTime) ? DateTime.Parse(maxCsend) : m.EndTime,
                     Description = m.Description,
                     DayOfWeek = DateTime.Parse(minCsstart).DayOfWeek
                 }).ToList();




            var res = new List<ScheduleViewModel>[7];
            for (int i = 0; i < 7; i++)
            {
                res[i] = new List<ScheduleViewModel>();
            }

        foreach (var work in workthis)
                {
                    if (work.Color != null || defaultcolor == null)
                        res[(int)work.DayOfWeek].Add(work);
                    else
                    {
                        ScheduleViewModel work2 = new ScheduleViewModel
                        {
                            Id = work.Id,
                            Color = defaultcolor.Color,
                            Name = work.Name,
                            Description = work.Description,
                            StartTime = work.StartTime,
                            EndTime = work.EndTime,
                            DayOfWeek = work.DayOfWeek

                        };
                        res[(int)work.DayOfWeek].Add(work2);
                    }
                }

 
            return res;

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

        public async Task<List<ScheduleViewModel>[]> GetActivitiesWithColour(ScheduleFormModel scheduleFormModel, string userid)
        {
            ColorActivity defaultcolor;

            var colorfromthis = _ctx.ColorActivity
                                 .Where(
                                 cm =>
                                 (cm.LMSUserId ?? userid) == userid
                                 && cm.LMSActivityId != null
                                 );

            var colorfromtype = _ctx.ColorActivity
                                 .Where(
                                 cm =>
                                 (cm.LMSUserId ?? userid) == userid
                                 && cm.LMSActivityId == null
                                 && cm.AktivityTypeID!= null
                                 );

            defaultcolor =await  _ctx.ColorActivity
                            .Where(
                            cm =>
                            (cm.LMSUserId ?? userid) == userid
                            && cm.LMSActivityId == null
                            && cm.AktivityTypeID == null
                            ).FirstOrDefaultAsync();

            /* var testc = await _ctx.Modules
                .Include(m => m.LMSActivities)
                .ThenInclude(a => a.ActivityType)
                .Where(m => m.CourseId.ToString() == scheduleFormModel.CourseId
                  && ((m.EndDate >= scheduleFormModel.StartTime)
                                && (m.StartDate <= scheduleFormModel.EndTime)))

                .SelectMany(m => m.LMSActivities).ToListAsync();*/
            var coursesettings = await GetCourseSettingsAsync(scheduleFormModel.CourseId, scheduleFormModel.StartTime, scheduleFormModel.EndTime);

            var minCsstart = coursesettings.Min(cs => cs.StartTime);
            var maxCsend = coursesettings.Max(cs => cs.EndTime);

            ICollection<LMSActivity> amongthese;
            try
            {
                amongthese = await _ctx.Modules
           .Include(m => m.LMSActivities)
           .ThenInclude(a => a.ActivityType)
           .Where(m => m.CourseId.ToString() == scheduleFormModel.CourseId
             && (m.EndDate >= scheduleFormModel.StartTime)
                           && (m.StartDate <= scheduleFormModel.EndTime))

           .SelectMany(m => m.LMSActivities).ToListAsync();

            ;
            }
            catch (Exception ex)
            {

                throw;
            }

            try
            {
                amongthese = amongthese
              .Where(a => (a.EndDate >= scheduleFormModel.StartTime )

              && (a.StartDate <= scheduleFormModel.EndTime ))
              
              
              .ToList();

                amongthese = amongthese//.Where(m => m.StartDate < scheduleFormModel.StartTime)
                    .Select
                    (
                     m => new LMSActivity
                     {
                         Id = m.Id,
                         Name = m.Name,
                         StartDate = (DateTime.Parse(minCsstart)> m.StartDate)? DateTime.Parse(minCsstart): m.StartDate,
                         EndDate = (DateTime.Parse(maxCsend) < m.EndDate)? DateTime.Parse(maxCsend):m.EndDate,
                         Description = m.Description,
                         ModuleId = m.ModuleId,
                         ActivityTypeId = m.ActivityTypeId

                     }


                    ).ToList();


            }
            catch (Exception ex)
            {

                throw;
            }

            

            List<ScheduleViewModel> workthis;

            if (colorfromthis.Count() > 0)
            {
                try
                {
                    workthis = amongthese
                            ?.GroupJoin(
                    colorfromthis,
                    m => m.Id,
                    cm => cm.Id,
                    (x, y) => new { mod = x, color = y }
                    )
                    .SelectMany(
                     x => x.color.DefaultIfEmpty(),
              (x, y) => new ScheduleViewModel
              {
                  Id = x.mod.Id.ToString(),
                  Color = y.Color,
                  Name = x.mod.Name,
                  Description = x.mod.Description,
                  StartTime = x.mod.StartDate,
                  EndTime = x.mod.EndDate,
                  DayOfWeek = x.mod.StartDate.DayOfWeek,
                  ActivityTypeId = x.mod.ActivityTypeId
              }).ToList();
                }
                catch (Exception ex)
                {

                    throw;
                }



                try
                {
                    /* update work with matches from colorfromtype */
        workthis = workthis
                             ?.GroupJoin(
                                colorfromtype,
                                m => m.ActivityTypeId,
                                cm => cm.AktivityTypeID,
                                (x, y) => new { mod = x, color = y }
                                )
                                .SelectMany(
                                 x => x.color.DefaultIfEmpty(),
                          (x, y) => new ScheduleViewModel
                          {
                              Id = x.mod.Id.ToString(),
                              Color = y.Color,
                              Name = x.mod.Name,
                              Description = x.mod.Description,
                              StartTime = x.mod.StartTime,
                              EndTime = x.mod.EndTime,
                              DayOfWeek = x.mod.DayOfWeek,
                              ActivityTypeId = x.mod.ActivityTypeId
                          }).ToList();

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            else
            {
                try
                {
                    /* update work with matches from colorfromtype */
                    workthis = amongthese
                             ?.GroupJoin(
                                colorfromtype,
                                m => m.ActivityTypeId,
                                cm => cm.AktivityTypeID,
                                (x, y) => new { mod = x, color = y }
                                )
                                .SelectMany(
                                 x => x.color.DefaultIfEmpty(),
                          (x, y) => new ScheduleViewModel
                          {
                              Id = x.mod.Id.ToString(),
                              Color = y.Color,
                              Name = x.mod.Name,
                              Description = x.mod.Description,
                              StartTime = x.mod.StartDate,
                              EndTime = x.mod.EndDate,
                              DayOfWeek = x.mod.StartDate.DayOfWeek,
                              ActivityTypeId = x.mod.ActivityTypeId
                          }).ToList();

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            var res = new List<ScheduleViewModel>[7];
            for (int i = 0; i < 7; i++)
            {
                res[i] = new List<ScheduleViewModel>();
            }
            try
            {
                foreach (var work in workthis)
                {
                    if (work.Color != null || defaultcolor== null)
                        res[(int)work.DayOfWeek].Add(work);
                    else
                    {
                        ScheduleViewModel work2 = new ScheduleViewModel
                        {
                            Id = work.Id,
                            Color = defaultcolor.Color,
                            Name = work.Name,
                            Description = work.Description,
                            StartTime = work.StartTime,
                            EndTime = work.EndTime,
                            DayOfWeek = work.DayOfWeek

                        };
                        res[(int)work.DayOfWeek].Add(work2);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            return res;

        }


        public async Task<bool> RemoveActivityHelperAsync(Guid activityid)
        {
            var docActivity = await _DocumentRepository.GetAllDocumentsForActivityAsync(activityid);
            return await _DocumentRepository.RemoveDocumentRangeAsync(docActivity.ToList());
        }

        public async Task<ICollection<ActivityViewModel>> GetActivitiesFromModulid(Guid moduleId)
        {

            var Activities = await _ctx.LMSActivity
                           .Include(a => a.ActivityType)
                          .Where(a => a.ModuleId == moduleId)
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
            return res;
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
 