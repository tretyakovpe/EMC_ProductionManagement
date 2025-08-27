using Microsoft.AspNetCore.SignalR;
using ProductionManagement.Hubs;

namespace ProductionManagement.Services;

public class LoggerService
{
    private readonly ILogger _logger;
    private readonly IHubContext<LogHub> _hubContext;
    private readonly string _logFilePath;

    public LoggerService(ILogger<LoggerService> logger, IHubContext<LogHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
        _logFilePath = $"./console/{DateTime.Now:yyyyMMdd-HHmmss}.txt";
    }

    public void SendLog(string message)
    {
        var logMessage = $"{DateTime.Now.ToString("HH:mm:ss")} - {message}";
        _logger.LogInformation(logMessage);
        _hubContext.Clients.All.SendAsync("ReceiveLog", logMessage);
    }

    public void SendLog(string message, string type)
    {
        string logMessage = $"{DateTime.Now:yyyyMMdd HH:mm:ss} - {message}";
        switch (type)
        {
            case "info":
                _logger.LogInformation(logMessage);
                _hubContext.Clients.All.SendAsync("ReceiveLog", logMessage);
                break;
            case "warn":
                _logger.LogWarning(logMessage);
                _hubContext.Clients.All.SendAsync("ReceiveLog", logMessage);
                break;
            case "error":
                _logger.LogError(logMessage);
                _hubContext.Clients.All.SendAsync("ReceiveLog", logMessage);
                break;
            default:
                break;
        }
        try
        {
            File.AppendAllText(_logFilePath,logMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при записи в файл логов.");
        }
    }
}