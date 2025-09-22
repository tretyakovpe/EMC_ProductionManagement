using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductionManagement.Models;
using ProductionManagement.Services;

namespace ProductionManagement.Controllers
{
    public class PartNokController : Controller
    {
        private readonly PartsManager _partsManager;
        private readonly IWebHostEnvironment _env;

        public PartNokController(PartsManager partsManager, IWebHostEnvironment env)
        {
            _partsManager = partsManager;
            _env = env;
        }

        // GET: PartNok
        public async Task<IActionResult> Index()
        {
            // Получаем последние 50 дефектных деталей
            var last50NokParts = await _partsManager.GetLatest50PartNoksAsync();

            // Проверяем наличие файлов видео и устанавливаем признак наличия
            foreach (var part in last50NokParts)
            {
                if (part.Video != null)
                {
                    var videoPath = Path.Combine(_env.ContentRootPath, "video", part.Video);
                    part.FileExists = System.IO.File.Exists(videoPath);
                }
                else
                {
                    part.FileExists = false;
                }
            }
            return View(last50NokParts);
        }

        // GET: PartNok/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Получаем запись PartNok по ID
            var part = await _partsManager.GetPartNokByIdAsync(id);

            // Готовим JSON с состояниями активных MKM
            var mkmJson = GenerateActiveMkmJson(part.Mkm);

            // Передаём данные в представление
            return View(new Tuple<PartNok, string>(part, mkmJson));
        }

        // Генерируем JSON с состояниями активных MKM
        private string GenerateActiveMkmJson(byte[] mkm)
        {
            // Читаем JSON-маркировку
            var mapping = ReadMappingFile();

            // Собираем активные биты
            var activeBits = new List<MKMMapping>();

            for (int i = 0; i < mkm.Length; i++)
            {
                var bits = Enumerable.Range(0, 8).Select(b => (mkm[i] & (1 << b)) != 0).Reverse().ToArray();

                for (int j = 0; j < bits.Length; j++)
                {
                    if (bits[j])
                    {
                        var addr = $"{i + 26}.{j}"; // Адрес бита
                        var mappingEntry = mapping.FirstOrDefault(x => x.Address == addr);

                        if (mappingEntry != null)
                        {
                            activeBits.Add(mappingEntry);
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(activeBits);
        }

        // Читаем файл разметки MKM
        private IEnumerable<MKMMapping> ReadMappingFile()
        {
            var json = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "mkm.json"));
            return JsonConvert.DeserializeObject<IEnumerable<MKMMapping>>(json);
        }

        // Показ видео во всплывающем окне
        public ActionResult ShowVideo(string videoFilename)
        {
            var videoPath = Path.Combine(_env.ContentRootPath, "video", videoFilename);

            if (!System.IO.File.Exists(videoPath))
            {
                return NotFound();
            }

            // Читаем файл как поток
            var stream = new FileStream(videoPath, FileMode.Open, FileAccess.Read);

            // Указываем Content-Type как "video/mp4"
            return new FileStreamResult(stream, "video/mp4");
        }

        // Метод для скачивания видео
        public IActionResult DownloadVideo(string videoFilename)
        {
            // Проверяем существование файла
            var videoPath = Path.Combine(_env.ContentRootPath, "video", videoFilename);
            if (!System.IO.File.Exists(videoPath))
            {
                return NotFound(); // Если файл не найден, возвращаем 404
            }

            // Устанавливаем заголовки для скачивания
            var mimeType = "video/mp4";
            var fileName = Path.GetFileName(videoPath);

            // Возвращаем файл с предложением скачать
            return PhysicalFile(videoPath, mimeType, fileName);
        }
    }
}