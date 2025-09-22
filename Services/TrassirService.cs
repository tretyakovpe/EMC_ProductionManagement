using System.Text.Json;

namespace ProductionManagement.Services
{
    public class TrassirService
    {
        private readonly LoggerService _logger;
        private readonly string _serverAddress;
        private readonly string _username;
        private readonly string _password;
        private readonly HttpClient _client;

        public TrassirService(string serverAddress, string username, string password, LoggerService logger)
        {
            _logger = logger;
            _serverAddress = serverAddress;
            _username = username;
            _password = password;
            // Конфигурация HttpClient для игнорирования проверки сертификата
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
            };
            _client = new HttpClient(handler);
        }

        public async Task SaveVideoAsync(string channelGuid, DateTime moment)
        {
            // Шаг 1: Получаем session ID
            var response = await _client.GetAsync($"{_serverAddress}login?password={_password}");
            var content = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(content);
            var sessionId = doc.RootElement.TryGetProperty("sid", out var sidProp) ? sidProp.GetString() : null;
            if (sessionId == null)
            {
                _logger.SendLog("Не удалось получить session ID", "error");
                return;
            }

            // Шаг 2: Вычисляем временные границы: минус 1 минуту до момента и плюс 30 секунд после
            long unixMicrosecondsPerSecond = 1000000L;
            long microsecondOffsetBefore = 60 * unixMicrosecondsPerSecond; // 1 минута назад
            long microsecondOffsetAfter = 30 * unixMicrosecondsPerSecond; // 30 секунд вперед

            long startTs = ((long)(moment.Subtract(TimeSpan.FromSeconds(60)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds * unixMicrosecondsPerSecond);
            long endTs = ((long)(moment.AddSeconds(30).Subtract(new DateTime(1970, 1, 1))).TotalSeconds * unixMicrosecondsPerSecond);

            // Шаг 3: Создание задачи экспорта видео
            var exportTaskRequest = new
            {
                resource_guid = channelGuid,
                start_ts = startTs,
                end_ts = endTs,
                is_hardware = 0,
                prefer_substream = 0
            };
            var json = JsonSerializer.Serialize(exportTaskRequest);
            var content1 = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var createTaskResponse = await _client.PostAsync($"{_serverAddress}jit-export-create-task?sid={sessionId}", content1);
            var createTaskContent = await createTaskResponse.Content.ReadAsStringAsync();
            var taskDoc = JsonDocument.Parse(createTaskContent);
            if (!taskDoc.RootElement.TryGetProperty("task_id", out var taskIdProp))
            {
                _logger.SendLog("Не удалось получить task_id", "error");
                return;
            }
            var taskId = taskIdProp.GetString();
            var downloadUrl = $"{_serverAddress}jit-export-download?sid={sessionId}&task_id={taskId}";

            // Шаг 4: Скачиваем видео
            var responseDownload = await _client.GetAsync(downloadUrl);
            byte[] videoData = await responseDownload.Content.ReadAsByteArrayAsync();
            if (videoData.Length == 0)
            {
                _logger.SendLog("Видео не скачалось", "error");
                return;
            }

            // Шаг 5: Сохраняем файл
            try
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "video")); // Создаем директорию, если её нет
                string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "video", $"{channelGuid}-{moment:Ticks}.mp4");
                File.WriteAllBytes(filename, videoData);
                _logger.SendLog($"Видео сохранено: {filename}");
            }
            catch (Exception ex)
            {
                _logger.SendLog($"Ошибка при сохранении видео: {ex}", "error");
            }
        }
    }
}