using TaskAssignmentSystem.Models.Config;
using TaskAssignmentSystem.Services.Interfaces;
using TaskAssignmentSystem.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);


// ?? Bind AppSettings section from appsettings.json
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Our app services (in-memory for Phase 1)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IWorkspaceService, WorkspaceService>();

// (You already had these; keep if you need demo TaskService)
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
