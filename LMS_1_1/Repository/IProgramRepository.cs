using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS_1_1.Models;
using LMS_1_1.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LMS_1_1.Repository
{
    public interface IProgramRepository
    {

        Task<IEnumerable<Course>> GetAllCoursesAsync (bool includeModule);
        Task<Course> GetCourseByIdAsync (Guid courseId, bool includeModule);
        Task<bool> CourseExistsAsync (Guid id);
        /* Course FindCourseById (string courseId);*/
        string GetCourseImageUploadPath ();

        Task<IEnumerable<Module>> GetAllModulesAsync (bool includeActivities);
        Task<Module> GetModuleByIdAsync (Guid moduleId, bool includeActivity);
        Task<bool> ModuleExistsAsync (Guid id);
        Task<IEnumerable<Course>> GetCoursesForUserAsync(string userid);


        Task<IEnumerable<LMSActivity>> GetAllActivitiesAsync ();
        Task<LMSActivity> GetActivityByIdAsync (Guid activityId);
        Task<bool> LMSActivityExistsAsync (Guid id);
        Task<bool> MoveLMSActivity(ActivityFormModel modelVm);

        Task<IEnumerable<ActivityType>> GetAllActivityTypesAsync ();
        Task<ActivityType> GetAllActivityTypesByIdAsync (int activityTypeId);
        Task<bool> ActivityTypeExistsAsync (int id);

        Task AddTokenUser(string token, string userid);
        Task<bool> RemoveTokenUser(string token);
        Task<bool> IsTeacher(string token);
        Task<bool> RemoveCourseHelperAsync(Guid courseid);
        Task<bool> RemoveModuleHelperAsync(Guid moduleid);
        Task<bool> RemoveActivityHelperAsync(Guid activityid);
        Task<bool> MoveModule(ModuleViewModel modelVm);
        Task<bool> CheckIfModuleInRange(string courseid, DateTime start, DateTime end);
        Task<bool> CheckIfActivityInRange(string modulid, DateTime start, DateTime end);
        Task<bool> SaveAllAsync ();
        Task AddEntityAsync (object model);
        void UpdateEntity (object model);
        void RemoveEntity (object model);
        Task<Course> CloneCourseAsync(CloneFormModel cloneFormModel, string userid);
    }
}
