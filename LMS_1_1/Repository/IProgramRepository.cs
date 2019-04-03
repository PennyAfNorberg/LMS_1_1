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
        Task<Course> GetCourseByIdAsync (Guid courseId);
        Task<CourseAllViewModel> GetCourseByIdAllAsync(Guid courseId);
        Task<CourseAllViewModel> GetCourseAndModuleAsync(Guid courseId);

        Task<bool> CourseExistsAsync (Guid id);
        /* Course FindCourseById (string courseId);*/
        string GetCourseImageUploadPath ();

        Task<IEnumerable<Module>> GetAllModulesAsync (bool includeActivities);
        Task<Module> GetModuleByIdAsync (Guid moduleId, bool includeActivity);

        Task<ModelAllViewModel> GetModulesAndActivitiesFromModulidAsync(Guid moduleId);
        Task<bool> ModuleExistsAsync (Guid id);
        Task<IEnumerable<Course>> GetCoursesForUserAsync(string userid);


        Task<IEnumerable<LMSActivity>> GetAllActivitiesAsync ();
        Task<LMSActivity> GetActivityByIdAsync (Guid activityId);
        Task<ICollection<ActivityViewModel>> GetActivitiesFromModulidAsync(Guid moduleId);
        Task<bool> LMSActivityExistsAsync (Guid id);
        Task<bool> MoveLMSActivityAsync(ActivityFormModel modelVm);

        Task<IEnumerable<ActivityType>> GetAllActivityTypesAsync ();
        Task<ActivityType> GetAllActivityTypesByIdAsync (int activityTypeId);
        Task<bool> ActivityTypeExistsAsync (int id);
        
        Task AddTokenUserAsync(string token, string userid);
        Task<bool> RemoveTokenUserAsync(string token);
        Task<bool> IsTeacherAsync(string token);
        Task<bool> RemoveCourseHelperAsync(Guid courseid);
        Task<bool> RemoveModuleHelperAsync(Guid moduleid);
        Task<ActivityViewModel> GetActivityByIdWithColorAsync(Guid idG, string userid);
        Task<bool> RemoveActivityHelperAsync(Guid activityid);
        Task<bool> MoveModuleAsync(ModuleViewModel modelVm);
        Task<bool> CheckIfModuleInRangeAsync(string courseid, DateTime start, DateTime end);
        Task<bool> CheckIfActivityInRangeAsync(string modulid, DateTime start, DateTime end);
        Task<bool> SaveAllAsync ();
        Task AddEntityAsync (object model);
        void UpdateEntity (object model);
        void RemoveEntity (object model);
        Task<Course> CloneCourseAsync(CloneFormModel cloneFormModel, string userid);
        Task<List<ScheduleViewModel>[]> GetModulesWithColourAsync(ScheduleFormModel scheduleFormModel,string userid);
        Task<List<ScheduleViewModel>[]> GetActivitiesWithColourAsync(ScheduleFormModel scheduleFormModel, string userid);
        List<CourseSettingsViewModel> GetCourseSettings(string courseId, DateTime? startDate, DateTime? endDate);
    }
}
