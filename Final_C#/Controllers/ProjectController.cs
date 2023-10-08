using AdvancedTopic_FinalProject.Areas.Identity.Data;
using AdvancedTopic_FinalProject.Data;
using AdvancedTopic_FinalProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using X.PagedList;

using Project = AdvancedTopic_FinalProject.Models.Project;

namespace AdvancedTopic_FinalProject.Controllers
{
    [Authorize(Roles = "Project Manager")]
    public class ProjectController : Controller
    {

        private readonly ILogger<ProjectController> _logger;
        private readonly TaskManagementContext _context;
        private readonly UserManager<TaskUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<TaskUser> _signInManager;

        public ProjectController(ILogger<ProjectController> logger, TaskManagementContext context, RoleManager<IdentityRole> roleManager, UserManager<TaskUser> userManager, SignInManager<TaskUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Project Manager")]


        public IActionResult Index(string? sortBy, int? sortId, int? page)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            //var projects = _context.Projects.Include(project => project.Tasksids).Where(p=> p.DemoUserID == userId).OrderBy(project => project.title).ToList();
            var pageNumber = page ?? 1; 
            var pageSize = 10; 

            var projects = _context.Projects
               .Include(project => project.Tasks)
               .Where(p => p.DemoUserID == userId)
               .OrderBy(project => project.title)
               .ToPagedList(page ?? 1, 10);
            ViewBag.SortBy = sortBy;
            ViewBag.SortId = sortId;

            var projectUserNames = new Dictionary<int, List<string>>();

            foreach (var project in projects)
            {
                var userIds = _context.DemoUserProjects
                    .Where(du => du.ProjectId == project.Id)
                    .Select(du => du.UserId)
                    .ToList();

                var userNames = _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => u.UserName)
                    .ToList();

                projectUserNames[project.Id] = userNames;
            }

            ViewBag.ProjectUserNames = projectUserNames;



            var taskUserNames = new Dictionary<int, List<string>>();
            var tasks = _context.Taasks.ToList();

            foreach (var taask in tasks)
            {
                var userIds = _context.DemoUserTasks
                    .Where(du => du.TaaskId == taask.Id)
                    .Select(du => du.RoleId)
                    .ToList();

                var userNames = _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => u.UserName)
                    .ToList();

                taskUserNames[taask.Id] = userNames;
            }
            ViewBag.taskUserNames = taskUserNames;


            if (string.IsNullOrEmpty(sortBy))
            {
               
            }
            else if (sortBy == "RequiredHours")
            {
                if (sortId.HasValue)
                {
                    
                    var projectToSort = projects.FirstOrDefault(project => project.Id == sortId.Value);

                    if (projectToSort != null)
                    {
                       
                        projectToSort.Tasks = new HashSet<Taask>(
                            projectToSort.Tasks.OrderBy(task => task.RequiredHours)
                        );
                    }
                }
            }
            else if (sortBy == "Priority")
            {
                if (sortId.HasValue)
                {
                    
                    var projectToSort = projects.FirstOrDefault(project => project.Id == sortId.Value);

                    if (projectToSort != null)
                    {
                        
                        projectToSort.Tasks = new HashSet<Taask>(
                            projectToSort.Tasks.OrderBy(task => task.Priority)
                        );
                    }
                }
            }

            return View(projects);
        }


        public IActionResult Create()
        {


            var RoleD = _roleManager.Roles.FirstOrDefault(r => r.Name == "Developer");
            var developers = _context.UserRoles
                .Where(userRole => userRole.RoleId == RoleD.Id)
                .ToList();
            var userIds = developers.Select(userRole => userRole.UserId).ToList();
            var users = _context.Users.Where(user => userIds.Contains(user.Id)).ToList();

            ViewBag.Users = users;

            return View(new Project());
        }

        [HttpPost]

        public async Task<IActionResult> Create(Project project, List<string> AreChecked)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Determine if the userid exists
            var userExists = _userManager.Users.Any(u => u.Id == userId);

            if (!userExists)
            {


                //If the userId does not exist, log out of the current account
                await HttpContext.SignOutAsync();

                // back to HomeController
                return RedirectToAction("Index", "Home");
            }

            project.DemoUserID = userId;

            if (!ModelState.IsValid)
            {
                var RoleD = _roleManager.Roles.FirstOrDefault(r => r.Name == "Developer");
                var developers = _context.UserRoles
                    .Where(userRole => userRole.RoleId == RoleD.Id)
                    .ToList();
                var userIds = developers.Select(userRole => userRole.UserId).ToList();
                var users = _context.Users.Where(user => userIds.Contains(user.Id)).ToList();

                ViewBag.Users = users;

                return View(project);  
            }
            else
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                foreach (var selectedUserId in AreChecked)
                {
                    var demoUserProject = new DemoUserProject
                    {
                        UserId = selectedUserId,
                        ProjectId = project.Id
                    };

                    _context.DemoUserProjects.Add(demoUserProject);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p=>p.Id== id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p=>p.Id== id);

            return View(project);
        }


        public async Task<IActionResult> AddToDeveloper(int id)
        {
            var userIds = _context.DemoUserProjects
                .Where(du => du.ProjectId == id)
                .Select(du => du.UserId)
                .ToList();

            var RoleId = _roleManager.Roles.FirstOrDefault(r => r.Name == "Developer");

            var developers = _context.UserRoles
                .Where(userRole => userRole.RoleId == RoleId.Id)
                .Select(userRole => userRole.UserId)
                .ToList();

            
            var usersNotInProject = developers.Except(userIds).ToList();


            List<TaskUser> usersList = new List<TaskUser>();

            foreach (var userId in usersNotInProject)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    usersList.Add(user);
                }
            }
            ViewBag.projectName = _context.Projects.FirstOrDefault(p => p.Id == id);
            ViewBag.UsersNotInProject = usersList;
            ViewBag.PId = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToDeveloper(int Pid, List<string> AreChecked)
        {

            foreach (var selectedUserId in AreChecked)
            {
                var demoUserProject = new DemoUserProject
                {
                    UserId = selectedUserId,
                    ProjectId = Pid
                };

                _context.DemoUserProjects.Add(demoUserProject);
            }

            await _context.SaveChangesAsync();



            return RedirectToAction("Index");
        }


        public async Task<IActionResult> AddToTask(int id, int Taskid)
        {
            var userids = _context.DemoUserTasks
                .Where(du => du.TaaskId == Taskid)
                .Select(du => du.RoleId)
                .ToList();


            var userProjects = _context.DemoUserProjects
                .Where(du => du.ProjectId == id)
                .ToList();

            

            var userIds = userProjects.Select(du => du.UserId).ToList();


            var usersNotInTask = userIds.Except(userids).ToList();


            var users = _context.Users
                .Where(u => usersNotInTask.Contains(u.Id))
                .ToList();

            ViewBag.TId = Taskid;
            ViewBag.TaskName = _context.Taasks.FirstOrDefault(p => p.Id == Taskid);

            ViewBag.users = users;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddToTask(int Tid, List<string> AreChecked)
        {

            foreach (var selectedUserId in AreChecked)
            {
                var demoUserTasks = new DemoUserTask
                {
                    RoleId = selectedUserId,
                    TaaskId = Tid
                };

                _context.DemoUserTasks.Add(demoUserTasks);
            }

            await _context.SaveChangesAsync();



            return RedirectToAction("Index");
        }

    }
}
