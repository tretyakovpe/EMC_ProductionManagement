using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;
using ProductionManagement.Services;
using ProductionManagement.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ����������� ��������� ���� ������
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();
builder.Services.AddHostedService<LinesPollingService>();
builder.Services.AddHostedService<LinesManagerService>();
builder.Services.AddControllersWithViews();

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

app.UseRouting();
app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseStaticFiles();
app.UseAuthorization();

// ���-��������� ����������� ���������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Lines}/{action=Index}/{id?}"
);

// ����������� ���� SignalR
app.MapHub<LogHub>("/loghub");

app.Run();