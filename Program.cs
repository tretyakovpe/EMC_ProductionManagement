using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionManagement.Data;
using ProductionManagement.Hubs;
using ProductionManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������������
//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// ����������� ��������� ���� ������
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����������� SignalR
builder.Services.AddSignalR();

// ����������� Background Services
builder.Services.AddHostedService<LinesManagerService>();
builder.Services.AddHostedService<LinesPollingService>();
builder.Services.AddSingleton<LinesPollingService>();
builder.Services.AddSingleton<LinesManagerService>();
builder.Services.AddScoped<LabelService>();
builder.Services.AddScoped<PlcService>();
builder.Services.AddTransient<LoggerService>();

// ����������� ������������ � �������������
builder.Services.AddControllersWithViews();

// ���������� ������������ ��� ��������� ����������
builder.Services.Configure<PollingSettings>(builder.Configuration.GetSection("PollingSettings"));

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