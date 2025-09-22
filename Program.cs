using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;
using ProductionManagement.Hubs;
using ProductionManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Подключаем конфигурацию для работы в качестве службы Windows
builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "ProductionManagement";// Имя службы
});
//Используем внутренний сервер Kestrel
builder.WebHost.ConfigureKestrel(kestrelOptions =>
{
    kestrelOptions.ListenAnyIP(80); // Порт 80 открыт для любых IP
    kestrelOptions.ListenAnyIP(443); // Порт 443 открыт для любых IP
});

// Настройки конфигурации
//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// Регистрация контекста базы данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Регистрация SignalR
builder.Services.AddSignalR();

// Регистрация сервисов, работающих только в production-среде
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<LinesManagerService>();
    builder.Services.AddHostedService<LinesPollingService>();
}
// Регистрация Background Services
//builder.Services.AddHostedService<LinesManagerService>();
//builder.Services.AddHostedService<LinesPollingService>();
builder.Services.AddScoped<LinesPollingService>();
builder.Services.AddScoped<LinesManagerService>();
builder.Services.AddScoped<LabelService>();
builder.Services.AddScoped<PlcService>();
builder.Services.AddScoped<PartsManager>();
builder.Services.AddTransient<LoggerService>();
builder.Services.AddScoped<TrassirService>(
    sp => new TrassirService(
        "https://togcctv.emc-tlt.tech:8080/",
        "Script",
        "99015477",
         sp.GetRequiredService<LoggerService>())
);

// Регистрация контроллеров и представлений
builder.Services.AddControllersWithViews();

// Добавление конфигурации для временных интервалов
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

// Топ-уровневая регистрация маршрутов
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Lines}/{action=Index}/{id?}"
);

// Регистрация хаба SignalR
app.MapHub<LogHub>("/loghub");

await app.RunAsync();