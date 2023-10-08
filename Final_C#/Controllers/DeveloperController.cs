
using AdvancedTopic_FinalProject.Models.ViewModel;
using AdvancedTopic_FinalProject.Areas.Identity.Data;
using AdvancedTopic_FinalProject.Data;
using AdvancedTopic_FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AdvancedTopic_FinalProject.Controllers
{
    [Authorize(Roles = "Developer")]
    public class DeveloperController : Controller
    {
        private readonly ILogger<DeveloperController> _logger;
        private readonly TaskManagementContext _context;
        private readonly UserManager<TaskUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<TaskUser> _signInManager;

        public DeveloperController(ILogger<DeveloperController> logger, TaskManagementContext context, RoleManager<IdentityRole> roleManager, UserManager<TaskUser> userManager, SignInManager<TaskUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

       
        public async Task <IActionResult> Index()

        {
          
            return View();
        }
        public async Task<IActionResult> ProjectsAssigned()

        {
            var CurrentUserId = _userManager.GetUserId(User);
            var userProjects = _context.DemoUserProjects
                                           .Include(d => d.Project)
                                           .Where(p => p.UserId == CurrentUserId)
                                           .Select(d => d.Project)
                                           .ToList();

            List<ProjectTaskViewModel> ProjectTask = new List<ProjectTaskViewModel>();

            foreach (var project in userProjects)
            {
                var assignedTask = _context.DemoUserTasks
                                           .Where(t => t.RoleId == CurrentUserId && t.Taask.ProjectId == project.Id)
                                           .Select(d => d.Taask).ToList();

                ProjectTask.Add(new ProjectTaskViewModel
                {
                   Project = project,
                    UserTasks = assignedTask

                });
            }
            return View(ProjectTask);
        }



        public async Task<IActionResult> EditTask(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Task = await _context.Taasks.FindAsync(id);

            if (Task == null)
            {
                return NotFound();
            }

            EditTaskViewModel vm = new EditTaskViewModel
            {
                Id = Task.Id,
                RequiredHours = Task.RequiredHours,

            };
         
            return View(vm);
        }

        // POST: Person/Edit/5
        [HttpPost]
        
        public async Task<IActionResult> EditTask(int id, [Bind("Id, RequiredHours")] EditTaskViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                Taask task = await _context.Taasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound();
                }
                task.RequiredHours = vm.RequiredHours;
                _context.Update(task);
                _context.SaveChanges();

               
                return RedirectToAction(nameof(ProjectsAssigned));
            }
            return View(vm);
        }
        private bool TaskExists(int id)
        {
            return _context.Taasks.Any(e => e.Id == id);
        }

        public IActionResult CompletedTask(int id, bool isCompleted)
        {
            Taask? task = _context.Taasks.FirstOrDefault(i => i.Id == id);

            if (task != null)
            {
                task.CompletedTask = isCompleted;
                _context.SaveChanges();
            }

            return RedirectToAction($"ProjectsAssigned");
        }
    }
}
