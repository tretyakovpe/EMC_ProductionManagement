using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductionManagement.Data;
using ProductionManagement.Hubs;
using ProductionManagement.Models;
using Sharp7;
using System.Text;
using static ProductionManagement.Services.PlcService.StatusType;

namespace ProductionManagement.Services;

public class LinesPollingService : BackgroundService
{
    private readonly LinesManagerService _linesManagerService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly LoggerService _logger;
    private readonly PollingSettings _pollingSettings;

    private PeriodicTimer? _timer;
    private bool _isStarted;

    public LinesPollingService(
        IServiceScopeFactory scopeFactory,
        LoggerService logger,
        IHubContext<LogHub> hubContext,
        IOptions<PollingSettings> pollingSettings,
        LinesManagerService linesManagerService)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _pollingSettings = pollingSettings.Value;
        _linesManagerService = linesManagerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.SendLog("Lines Polling Service is starting.","info"); // Логирование начала работы
        _isStarted = true;
        // Задержка перед первым опросом
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_pollingSettings.PlcPollingIntervalInMilliseconds));

        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                //_logger.LogInformation("Lines Polling Service TIMER"); // Логирование начала работы
                await PollLinesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.SendLog($"{ex}, ошибка службы Lines Polling.","error");
            }
        }
        _isStarted = false;
    }
    public bool IsStarted => _isStarted;

    private async Task PollLinesAsync(CancellationToken stoppingToken)
    {
        var activeLines = LinesData.LinesCache.Values.ToList();

        foreach (var line in activeLines)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            await PollSingleLineAsync(line);
        }
    }

    private async Task PollSingleLineAsync(Line line)
    {
        using var scope = _scopeFactory.CreateScope();
        var plcService = scope.ServiceProvider.GetRequiredService<PlcService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            //_logger.SendLog($"Пытаемся прочитать линию {line.Name} - {line.Ip}");

            if (!plcService.Connect(line.Ip.Trim()))
            {
                //Говорим что линия оффлайн
                await dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.UpdateIsOnline @LineName, @IsOnline",
                    new SqlParameter("@LineName", line.Name),
                    new SqlParameter("@IsOnline", false)
                );
                return;
            }
            //Ставим лайвбит
            plcService.SetFlagAt(Livebit);

            //Говорим что линия онлайн
            await dbContext.Database.ExecuteSqlRawAsync(
                "EXEC dbo.UpdateIsOnline @LineName, @IsOnline",
                new SqlParameter("@LineName", line.Name),
                new SqlParameter("@IsOnline", true)
            );

            //Чтение данных о каждой детали
            var partdata = plcService.ReadDataBlock(1013, 0, 34);
            //Сбрасываем флаг "деталь" на линии
            plcService.SetFlagAt(Partready);
            if (partdata != null)
            {
                string partMaterial = S7.GetStringAt(partdata, 14);
                bool partReady = S7.GetBitAt(partdata, 2, 2);
                int counter = S7.GetIntAt(partdata, 32);
                if (partReady)
                {
                    // Логика обработки готовых данных
                    _logger.SendLog($"{line.Name} - {partMaterial} - {counter}");
                }
            }

            //Чтение данных о ящиках
            var db = plcService.ReadDataBlock(1012, 0, 64);
            if (db != null)
            {
                bool boxIsReady = S7.GetBitAt(db, 1, 0);
                string Material = S7.GetStringAt(db, 2);
                double Amount = S7.GetRealAt(db, 22);
                //=======Это извращение для чтения названия продукции. Она прилетает в ASCII
                string Material_Description = Encoding.GetEncoding(1251).GetString(db, 28, 36);
                //===================
                if (boxIsReady)
                {
                    //Сбрасываем флаг "ящик" на линии.
                    plcService.SetFlagAt(PlcService.StatusType.Boxready);
                    // Данные для передачи в процедуру
                    var date = DateTime.Now.Date;
                    var time = DateTime.Now.TimeOfDay;
                    var label = $"{line.Name.Trim()}{DateTime.Now:yyMMddHHmm}";
                    var material = Material;
                    var amount = (int)Amount;

                    // Логирование данных перед передачей
                    _logger.SendLog($"{line.Name} - {material} - {amount} шт. {label}","info");
                    if (amount != 0)
                    {
                        // Выполнение хранимой процедуры через контекст базы данных
                        await dbContext.Database.ExecuteSqlRawAsync(
                            "EXEC dbo.AddBox @Date, @Time, @labelNumber, @Name, @Material, @Amount",
                            new SqlParameter("@Date", date),
                            new SqlParameter("@Time", time),
                            new SqlParameter("@labelNumber", label),
                            new SqlParameter("@Name", line.Name),
                            new SqlParameter("@Material", material),
                            new SqlParameter("@Amount", amount)
                        );
                    }
                    if (line.PrintLabel)
                    {
                        try
                        {
                            var labelService = scope.ServiceProvider.GetRequiredService<LabelService>();
                            var materialDetails = dbContext.Materials.FirstOrDefault(m => m.MaterialCode == Material);
                            // Передача данных в LabelService для печати бирки
                            await labelService.GenerateAndPrintLabelAsync(new Prod
                            {
                                Date = date,
                                Time = time,
                                Line = line.Name,
                                Label = label,
                                Material = material,
                                Amount = amount,
                            }, Material_Description, materialDetails, line.Printer.Trim());
                        }
                        catch (Exception ex)
                        {
                            _logger.SendLog($"Ошибка создания и печати бирки в службе Line Polling {ex.Message}", "error");
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            _logger.SendLog($"Ошибка при опросе линии {line.Name} {ex.Message}","error");
        }
        finally
        {
            plcService.Disconnect();
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.SendLog("Lines Polling Service is stopping.","info");

        _timer?.Dispose();

        await base.StopAsync(cancellationToken);
    }
}