using Sharp7;

namespace ProductionManagement.Services;

public class PlcService : IDisposable
{
    public enum StatusType
    {
        Livebit,
        Boxready,
        Partready
    }
    private readonly LoggerService _logger;
    private readonly S7Client _plc;

    public PlcService(LoggerService logger)
    {
        _logger = logger;
        _plc = new S7Client();
    }

    /// <summary>
    /// Подключается к PLC.
    /// </summary>
    /// <param name="ipAddress">IP-адрес PLC.</param>
    /// <returns>True, если подключение успешно; иначе false.</returns>
    public bool Connect(string ipAddress)
    {
        try
        {
            int result = _plc.ConnectTo(ipAddress.Trim(), 0, 2);
            if (result == 0)
            {
                return true;
            }
            else
            {
                _logger.SendLog($"Ошибка подключения к PLC: {ipAddress}. : {_plc.ErrorText(result)}", "error");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.SendLog($"{ex} Исключение при подключении к PLC: {ipAddress}", "error");
            return false;
        }
    }

    /// <summary>
    /// Отключается от PLC.
    /// </summary>
    public void Disconnect()
    {
        if (_plc.Connected)
        {
            _plc.Disconnect();
        }
    }

    /// <summary>
    /// Читает данные из блока данных PLC.
    /// </summary>
    /// <param name="dbNumber">Номер блока данных.</param>
    /// <param name="startByte">Начальный байт.</param>
    /// <param name="length">Длина данных.</param>
    /// <returns>Массив байтов с данными или null в случае ошибки.</returns>
    public byte[] ReadDataBlock(int dbNumber, int startByte, int length)
    {
        try
        {
            byte[] buffer = new byte[length];
            int result = _plc.DBRead(dbNumber, startByte, length, buffer);

            if (result == 0)
            {
                //_logger.SendLog($"Успешное чтение данных из DB{dbNumber}.");
                return buffer;
            }
            else
            {
                //_logger.SendLog($"Ошибка чтения данных из DB{dbNumber}. Ошибка: {_plc.ErrorText(result)}");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.SendLog($"{ex.Message} Исключение при чтении данных из DB{dbNumber}.", "error");
            return null;
        }
    }
    /// <summary>
    /// Устанавливает флаг
    /// </summary>
    /// <param name="type">Указывает тип флага</param>
    /// <returns>True if success, False if failed</returns>
    public bool SetFlagAt(StatusType type)
    {
        int result=-1;
        byte[] f = new byte[1];

        switch (type)
        {
            case StatusType.Partready:
                S7.SetBitAt(f, 0, 2, false);
                try {result = _plc.DBWrite(1013, 2, 1, f);}
                catch (Exception ex){_logger.SendLog($"PartReady не сброшен {ex.Message}", "error");}
                break;
            case StatusType.Boxready:
                S7.SetBitAt(f, 0, 0, false);
                try { result = _plc.DBWrite(1012, 1, 1, f); }
                catch (Exception ex){ _logger.SendLog($"BoxReady не сброшен {ex.Message}", "error");}
                break;
            case StatusType.Livebit:
                S7.SetBitAt(f, 0, 0, true);
                try { result = _plc.DBWrite(1012, 0, 1, f); }
                catch (Exception ex){_logger.SendLog($"LiveBit не передан {ex.Message}", "error");}
                break;
            default:
                result = -1;
                break;
        }
        return result == 0;
    }

    /// <summary>
    /// Проверяет, подключен ли PLC.
    /// </summary>
    public bool IsConnected => _plc.Connected;

    /// <summary>
    /// Освобождает ресурсы.
    /// </summary>
    public void Dispose()
    {
        Disconnect(); // Вызываем Disconnect вместо Dispose
    }
}