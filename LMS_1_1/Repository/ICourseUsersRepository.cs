using LMS_1_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_1_1.Repository
{
    public interface ICourseUsersRepository
    {
        IQueryable<CourseUser> GetCourseUsers(string CourseId);

        IQueryable<CourseUser> GetCoursesForUsers(string LMSUserId);

        Task AddCourseUserAsync(string CouresID, string LMSUserid);

        Task RemoveCourseUserAsync(Guid CouresID, string LMSUserid);
       
        Task RemoveAllCourseUsersForCourseAsync(string CouresID);
        Task RemoveAllCourseUsersForUserAsync(string UserID);

        Task<ICollection<LMSUser>> GetUsersAsync(string courseId, bool choosed);
        Task<string> GetCourseNameAsync(string CourseId);
        Task<bool> SaveChangesAsync();
        Task<ICollection<Course>> GetCoursesOffAsync(string UserId);
        Task<ICollection<Course>> GetCoursesOnAsync(string UserId);
    }
}
