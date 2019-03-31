using LMS_1_1.Data;
using LMS_1_1.Models;
using LMS_1_1.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_1_1.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<LMSUser> _userManager;
        private readonly IProgramRepository _repository;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(IProgramRepository repository, ILogger<CoursesController> logger, ApplicationDbContext context, UserManager<LMSUser> userManager )
        {
            _repository = repository;
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public ActionResult<Guid> CourseStudent(Guid id)
        {

            return View(id);
        }

        // GET: Courses
        [Authorize]
        public async Task<IActionResult> Home(Guid id)
        {
            return View(await _repository.GetCourseByIdAllAsync(id));

        }
        // GET: Courses
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllCoursesAsync(true));

        }

        // GET: Courses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _repository.GetCourseByIdAllAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddEntityAsync(course);
                await _repository.SaveAllAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _repository.GetCourseByIdAsync(id);
            /* var course = await _context.Courses.FindAsync(id);*/
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,StartDate,Description")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                     _repository.UpdateEntity(course);
                    await _repository.SaveAllAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _repository.CourseExistsAsync(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _repository.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }


        // POST: Courses/Delete/5
        [Authorize(Roles = ConstDefine.R_TEACHER)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var course = await _repository.GetCourseByIdAsync(id);
            _repository.RemoveEntity(course);
            await _repository.SaveAllAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = ConstDefine.R_STUDENT)]
        public async Task<IActionResult> ShowStudent()
        {
            List<ShowStudent> stuCourses = new List<ShowStudent>();
            var userid = _userManager.GetUserId(User);

            var cs = from ucs in _context.CourseUsers where ucs.LMSUserId == userid select ucs;
            var lstcourse = await cs.ToListAsync();
            foreach (var item in cs)
            {
                var courseStu = new ShowStudent();
                var thecourse = await _repository.GetCourseByIdAsync(item.CourseId);
                courseStu.CourseID = item.CourseId;
                courseStu.CourseName = thecourse.Name;
                courseStu.StudentID = User.Identity.Name;
                stuCourses.Add(courseStu);
            }

            return View(stuCourses);
        }
    }

}