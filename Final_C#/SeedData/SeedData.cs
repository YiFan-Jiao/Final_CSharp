using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using AdvancedTopic_FinalProject.Data;
using AdvancedTopic_FinalProject.Areas.Identity.Data;

namespace AdvancedTopic_FinalProject.SeedData
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            TaskManagementContext context = new TaskManagementContext(serviceProvider.GetRequiredService<DbContextOptions<TaskManagementContext>>());
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<TaskUser> UserManager = serviceProvider.GetRequiredService<UserManager<TaskUser>>();

            // Ensure the database is properly set up
            context.Database.EnsureDeleted();
            context.Database.Migrate();

            // Seeding Roles
            if (!context.Roles.Any())
            {
                List<string> roles = new List<string> { "Administrator", "Project Manager", "Developer" };

                foreach (string s in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(s));
                }

                await context.SaveChangesAsync();
            }

            // Seeding User

            if (!context.Users.Any())
            {
                TaskUser Administrator = new TaskUser()
                {
                    UserName = "admin@admin.com",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@admin.com",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                TaskUser ProjectManagerOne = new TaskUser()
                {
                    UserName = "project@managerone.com",
                    NormalizedUserName = "MANAGERONE",
                    Email = "project@managerone.com",
                    NormalizedEmail = "PROJECT@MANAGERONE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                TaskUser ProjectManagerTwo = new TaskUser()
                {
                    UserName = "project@managertwo.com",
                    NormalizedUserName = "MANAGERTWO",
                    Email = "project@managertwo.com",
                    NormalizedEmail = "PROJECT@MANAGERTWO.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),

                };

                TaskUser ProjectManagerThree = new TaskUser()
                {
                    UserName = "project@managerthree.com",
                    NormalizedUserName = "MANAGERTHREE",
                    Email = "project@managerthree.com",
                    NormalizedEmail = "PROJECT@MANAGERTHREE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                TaskUser DevOne = new TaskUser()
                {
                    UserName = "dev@one.com",
                    NormalizedUserName = "DEVELOPERONE",
                    Email = "dev@one.com",
                    NormalizedEmail = "DEV@ONE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                TaskUser DevTwo = new TaskUser()
                {
                    UserName = "dev@two.com",
                    NormalizedUserName = "DEVELOPERTWO",
                    Email = "dev@two.com",
                    NormalizedEmail = "DEV@TWO.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                TaskUser DevThree = new TaskUser()
                {
                    UserName = "dev@three.com",
                    NormalizedUserName = "DEVELOPERTHREE",
                    Email = "dev@three.com",
                    NormalizedEmail = "DEV@THREE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                TaskUser DevFour = new TaskUser()
                {
                    UserName = "dev@four.com",
                    NormalizedUserName = "DEVELOPERFOUR",
                    Email = "dev@four.com",
                    NormalizedEmail = "DEV@FOUR.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                HashPasswordGenerator(Administrator, "Admin1!");
                HashPasswordGenerator(ProjectManagerOne, "ProjectManager1!");
                HashPasswordGenerator(ProjectManagerTwo, "ProjectManager2!");
                HashPasswordGenerator(ProjectManagerThree, "ProjectManager3!");
                HashPasswordGenerator(DevOne, "Developer1!");
                HashPasswordGenerator(DevTwo, "Developer2!");
                HashPasswordGenerator(DevThree, "Developer3!");
                HashPasswordGenerator(DevFour, "Developer4!");

                context.Users.Add(Administrator);
                context.Users.Add(ProjectManagerOne);
                context.Users.Add(ProjectManagerTwo);
                context.Users.Add(ProjectManagerThree);
                context.Users.Add(DevOne);
                context.Users.Add(DevTwo);
                context.Users.Add(DevThree);
                context.Users.Add(DevFour);

                await context.SaveChangesAsync();

                TaskUser adminUser = await UserManager.FindByNameAsync("Admin");
                TaskUser pmOneUser = await UserManager.FindByNameAsync("ManagerOne");
                TaskUser pmTwoUser = await UserManager.FindByNameAsync("ManagerTwo");
                TaskUser pmThreeUser = await UserManager.FindByNameAsync("ManagerThree");
                TaskUser devOne = await UserManager.FindByNameAsync("DeveloperOne");
                TaskUser devTwo = await UserManager.FindByNameAsync("DeveloperTwo");
                TaskUser devThree = await UserManager.FindByNameAsync("DeveloperThree");
                TaskUser devFour = await UserManager.FindByNameAsync("DeveloperFour");


                await UserManager.AddToRoleAsync(adminUser, "Administrator");
                await UserManager.AddToRoleAsync(pmOneUser, "Project Manager");
                await UserManager.AddToRoleAsync(pmTwoUser, "Project Manager");
                await UserManager.AddToRoleAsync(pmThreeUser, "Project Manager");
                await UserManager.AddToRoleAsync(devOne, "Developer");
                await UserManager.AddToRoleAsync(devTwo, "Developer");
                await UserManager.AddToRoleAsync(devThree, "Developer");
                await UserManager.AddToRoleAsync(devFour, "Developer");

                await context.SaveChangesAsync();
            }

            context.SaveChanges();

        }

        // Generating Hash Password
        public static string HashPasswordGenerator(TaskUser user, string password)
        {
            PasswordHasher<TaskUser> passwordHasher = new PasswordHasher<TaskUser>();
            string hashedPassword = passwordHasher.HashPassword(user, password);

            return user.PasswordHash = hashedPassword;
        }
    }
}
