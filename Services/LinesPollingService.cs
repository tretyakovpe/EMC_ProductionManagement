using Microsoft.AspNetCore.SignalR;
using ProductionManagement.Hubs;
using ProductionManagement.Models;
using Sharp7;

namespace ProductionManagement.Services;

public class LinesPollingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LinesPollingService> _logger;
    private readonly IHubContext<LogHub> _hubContext;
    private Timer _timer;

    public LinesPollingService(IServiceScopeFactory scopeFactory, ILogger<LinesPollingService> logger, IHubContext<LogHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        sendLog("Lines Polling Service is starting.");
        // Создаем таймер, который будет вызывать метод Poll каждые 1000 миллисекунд (одну секунду)
        _timer = new Timer(Poll, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10000));

        await Task.CompletedTask;
    }

    private void Poll(object state)
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var linesManagerService = scope.ServiceProvider.GetRequiredService<LinesManagerService>();
                var activeLines = linesManagerService.GetInternalStorage();
                foreach (var line in activeLines)
                {
                    sendLog($"Пытаемся прочитать линию {line.Name} - {line.Ip}");
                    S7Client plc = new();
                    int connResult = plc.ConnectTo(line.Ip.Trim(), 0, 2);

                    byte[] db = new byte[64];
                    byte[] partdata = new byte[34];

                    int result = plc.DBRead(1013, 0, 34, partdata);
                    if (result == 0)
                    {
                        string partMaterial = S7.GetStringAt(partdata, 14);
                        bool partReady = S7.GetBitAt(partdata, 2, 2);
                        int counter = S7.GetIntAt(partdata, 32);
                        bool partOK = S7.GetBitAt(partdata, 0, 0);
                        bool partNOK = S7.GetBitAt(partdata, 0, 1);
                        bool testStarted = S7.GetBitAt(partdata, 0, 2);
                        bool testFinished = S7.GetBitAt(partdata, 0, 3);
                        bool[] MKM = new bool[32];

                        sendLog($"{line.Name} - {partMaterial} - {counter}");

                        if (partReady)
                        {
                            // Обработчики логики
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while polling lines.");
        }
    }

    private void sendLog(string message)
    {
        _logger.LogInformation(message);
        _hubContext.Clients.All.SendAsync("ReceiveLog", message);
    }

    private void CheckLineStatus(Line line)
    {
        // Логика проверки состояния линии (опрашивание устройства)
        // Сохраняем изменения в базу данных
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Lines Polling Service is stopping.");

        // Останавливаем таймер перед завершением службы
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
}