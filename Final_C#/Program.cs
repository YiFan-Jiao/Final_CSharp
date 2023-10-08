using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdvancedTopic_FinalProject.Data;
using AdvancedTopic_FinalProject.Areas.Identity.Data;
using AdvancedTopic_FinalProject.SeedData;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ATAuthDemoContextConnection") ?? throw new InvalidOperationException("Connection string 'ATAuthDemoContextConnection' not found.");

builder.Services.AddDbContext<TaskManagementContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<TaskUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; 
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TaskManagementContext>();



builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();



var app = builder.Build();
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider serviceProvider = scope.ServiceProvider;

    await SeedData.Initialize(serviceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
