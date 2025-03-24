namespace ProductionManagement.Services;

using Microsoft.AspNetCore.SignalR;
using ProductionManagement.Data;
using ProductionManagement.Hubs;
using ProductionManagement.Models;
using System.Collections.Concurrent;

public class LinesManagerService : BackgroundService
{
    private static ConcurrentDictionary<string, Line> _linesCache = new ConcurrentDictionary<string, Line>();
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<LinesManagerService> _logger;
    private readonly IHubContext<LogHub> _hubContext;
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public LinesManagerService(ApplicationDbContext dbContext, ILogger<LinesManagerService> logger, IServiceProvider serviceProvider, IHubContext<LogHub> hubContext)
    {
        _dbContext = dbContext;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        sendLog($"Lines Fetching Service is starting.");

        // Запускаем таймер, который будет получать данные каждые 5 секунд
        _timer = new Timer(FetchLines, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    private void FetchLines(object state)
    {
        try
        {
            // Получаем список активных линий из базы данных
            List<Line> activeLines = _dbContext.Lines.Where(l => l.IsActive).ToList();

            // Обновляем внутреннее хранилище линий
            UpdateInternalStorage(activeLines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching lines.");
        }
    }

    private void UpdateInternalStorage(List<Line> lines)
    {
        // Очистка старых данных
        _linesCache.Clear();

        // Добавление новых данных
        foreach (var line in lines)
        {
            _linesCache.TryAdd(line.Name, line);
            sendLog($"Добавлена линия {line.Name}");
        }
    }

    // Получение актуальных данных
    public List<Line> GetInternalStorage()
    {
        return _linesCache.Values.ToList();
    }

    private void sendLog(string message)
    {
        _logger.LogInformation(message);
        _hubContext.Clients.All.SendAsync("ReceiveLog", message);
    }


    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Lines Fetching Service is stopping.");

        // Останавливаем таймер перед завершением службы
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
}