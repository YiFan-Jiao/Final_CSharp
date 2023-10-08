using AdvancedTopic_FinalProject.Areas.Identity.Data;
using AdvancedTopic_FinalProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTopic_FinalProject.Controllers;

[Authorize(Roles = "Administrator")]
public class UserController : Controller
{
    
    private readonly ILogger<UserController> _logger;
    private readonly TaskManagementContext _context;
    private readonly UserManager<TaskUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<TaskUser> _signInManager;

    public UserController(ILogger<UserController> logger, TaskManagementContext context, RoleManager<IdentityRole> roleManager, UserManager<TaskUser> userManager, SignInManager<TaskUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public async Task<IActionResult> Index()
    {
       
        var users = await _userManager.Users.ToListAsync();

        
        var userRoles = new Dictionary<string, IList<string>>();

        
        foreach (var user in users)
        {
           
            var roles = await _userManager.GetRolesAsync(user);

            
            userRoles[user.UserName] = roles;
        }

        
        ViewBag.UserRoles = userRoles;

        return View();
    }



    [HttpGet]
    public async Task<IActionResult> Edit(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return NotFound();
        }

        
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        
        var roles = await _userManager.GetRolesAsync(user);

        
        var allRoles = await _roleManager.Roles.ToListAsync();

        
        ViewData["User"] = user;
        ViewData["Roles"] = allRoles;
        ViewData["UserRoles"] = roles;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string username, List<string> selectedRoles)
    {
        
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            
            return NotFound();
        }

        try
        {
           
            var existingRoles = await _userManager.GetRolesAsync(user);

            
            var rolesToAdd = selectedRoles.Except(existingRoles);
            var rolesToRemove = existingRoles.Except(selectedRoles);

            
            foreach (var role in rolesToAdd)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            
            foreach (var role in rolesToRemove)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            
            return View("Error");
        }
    }


}
