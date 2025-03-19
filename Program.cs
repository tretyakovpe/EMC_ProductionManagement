using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;

var builder = WebApplication.CreateBuilder(args);
// ����������� IEndpointRouteBuilder
//builder.Services.AddSingleton<IEndpointRouteBuilder>(builder.Services.BuildServiceProvider().GetRequiredService<IEndpointRouteBuilder>());


// ����������� ��������� ���� ������
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����������� ����� MVC
builder.Services.AddControllersWithViews();

// ��������� �����������
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddFilter(level => level >= LogLevel.Information);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Lines}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Lines",
    pattern: "{controller=Lines}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Materials",
    pattern: "{controller=Materials}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "SiteMap",
    pattern: "{controller=SiteMap}/{action=Index}/{id?}");

app.Run();
