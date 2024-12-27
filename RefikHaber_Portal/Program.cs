using RefikHaber.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using RefikHaber.Repostories;
using Microsoft.AspNetCore.SignalR;
using RefikHaber.Hubs;
using RefikHaber.Mapping;
using RefikHaber_Portal.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UygulamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<UygulamaDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.AddScoped<HaberTuruRepository>();
builder.Services.AddScoped<HaberRepository>();
builder.Services.AddScoped<RoleManagerRepository>();

builder.Services.AddAutoMapper(typeof(MapProfile));

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapHub<GeneralHub>("/generalHub");

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
