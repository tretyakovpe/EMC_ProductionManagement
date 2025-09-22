using Microsoft.AspNetCore.SignalR;
using ProductionManagement.Hubs;

namespace ProductionManagement.Services;

public class LoggerService
{
    private readonly ILogger _logger;
    private readonly IHubContext<LogHub> _hubContext;
    private string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
    private readonly string _logFilePath;

    public LoggerService(ILogger<LoggerService> logger, IHubContext<LogHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
        _logFilePath = $"{_baseDir}/console/{DateTime.Now:yyyyMMdd}.txt";
    }

    /// <summary>
    /// Сохраняет сообщение в лог БЕЗ указания типа
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    public void SendLog(string message)
    {
        var logMessage = $"{DateTime.Now.ToString("HH:mm:ss")} - {message}\n\r";
        _logger.LogInformation(logMessage);
        _hubContext.Clients.All.SendAsync("ReceiveLog", logMessage);
    }

    /// <summary>
    /// Сохраняет сообщение в лог, с указанием типа.
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="type">тип может быть: info, warn, error</param>
    public void SendLog(string message, string type)
    {
        string logMessage = $"{DateTime.Now:yyyyMMdd HH:mm:ss} - {message}\n\r";
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
            File.AppendAllText(_logFilePath, logMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при записи в файл логов.");
        }
    }

    internal void UpdateCounter(string name, int counter, int volume)
    {
        _hubContext.Clients.All.SendAsync("UpdateCell", name, counter, volume);
    }
}