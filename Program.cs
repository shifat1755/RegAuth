using Microsoft.EntityFrameworkCore;
using RegAuth.Data;
using RegAuth.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("RegAuth")));
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("cookie").AddCookie("cookie", config => {
    config.Cookie.Name = "demo"; config.ExpireTimeSpan = TimeSpan.FromMinutes(30); config.LoginPath = "/User/Login"; });
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

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
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();

