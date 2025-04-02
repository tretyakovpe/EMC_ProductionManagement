using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using ProductionManagement.Data;
using ProductionManagement.Hubs;
using ProductionManagement.Models;

namespace ProductionManagement.Services;

public class LinesManagerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly LoggerService _logger;
    private readonly IHubContext<LogHub> _hubContext;
    private readonly PollingSettings _pollingSettings;
    private PeriodicTimer? _timer;

    public LinesManagerService(
        IServiceScopeFactory scopeFactory,
        LoggerService logger,
        IHubContext<LogHub> hubContext,
        IOptions<PollingSettings> pollingSettings)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _hubContext = hubContext;
        _pollingSettings = pollingSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.SendLog("Lines Manager Service is starting.");

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(_pollingSettings.LinesFetchIntervalInSeconds));

        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                FetchLines();
            }
            catch (Exception ex)
            {
                _logger.SendLog($"{ex} An error occurred while fetching lines.");
            }
        }
    }

    private void FetchLines()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Получаем список активных линий и удаляем лишние пробелы
            var activeLines = dbContext.Lines
                .Where(l => l.IsActive)
                .Select(l => new Line
                {
                    Name = l.Name.Trim(), // Удаляем пробелы
                    Ip = l.Ip.Trim(),
                    Port = l.Port,
                    Printer = l.Printer.Trim(),
                    PrintLabel = l.PrintLabel,
                    IsOnline = l.IsOnline,
                    LastCheck = l.LastCheck,
                    IsActive = l.IsActive
                })
                .ToList();
            // Обновляем внутреннее хранилище линий
            UpdateInternalStorage(activeLines);
        }
    }

    private void UpdateInternalStorage(List<Line> lines)
    {
        LinesData.LinesCache.Clear();
        foreach (var line in lines)
        {
            LinesData.LinesCache.TryAdd(line.Name, line);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.SendLog("Lines Manager Service is stopping.");

        _timer?.Dispose();

        await base.StopAsync(cancellationToken);
    }
}