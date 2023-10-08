using AdvancedTopic_FinalProject.Areas.Identity.Data;
using AdvancedTopic_FinalProject.Data;
using AdvancedTopic_FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTopic_FinalProject.Controllers
{
    [Authorize(Roles = "Project Manager")]
    public class TaskController : Controller
    {

        private readonly ILogger<TaskController> _logger;
        private readonly TaskManagementContext _context;
        private readonly UserManager<TaskUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<TaskUser> _signInManager;

        public TaskController(ILogger<TaskController> logger, TaskManagementContext context, RoleManager<IdentityRole> roleManager, UserManager<TaskUser> userManager, SignInManager<TaskUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(int projectId)
        {
            ViewData["ProjectId"] = projectId;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create([Bind("title,RequiredHours,Priority,ProjectId")] Taask tassk )
        {
            if (tassk == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return the user to the same view to correct the errors
                ViewData["ProjectId"] = tassk.ProjectId; // Ensure ProjectId is still available in the view
                return View(tassk);
            }

            _context.Taasks.Add(tassk);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Project");
        }

        public async Task<IActionResult> Delete(int id, int Taskid)
        {
           
            var taask = await _context.Taasks.FirstOrDefaultAsync(t => t.Id == Taskid && t.ProjectId == id);
            _context.Taasks.Remove(taask);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Project");
        }

    }
}
