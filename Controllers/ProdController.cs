using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;
using ProductionManagement.Models;

namespace ProductionManagement.Controllers
{
    public class ProdController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prod
        public async Task<IActionResult> Index(string line, string printer)
        {
            var prods = await _context.Prods
                .Where(p => p.Line == line)
                .OrderByDescending(p => p.Date)     // Первичная сортировка по дате в порядке убывания
                .ThenByDescending(p => p.Time)      // Дополнительная сортировка по времени в порядке убывания
                .ToListAsync();
            // Проходим по каждому элементу и проверяем существование файла
            foreach (var prod in prods)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "pdf", $"{prod.Label}.pdf");
                prod.FileExists = System.IO.File.Exists(filePath);
            }

            // Создаем анонимный объект и передаем в представление
            return View(Tuple.Create(prods.AsEnumerable(), printer));
        }

        //GET: Last50
        public async Task<IActionResult> Last50(string line, string printer)
        {
            var prods = await _context.Prods
                .Where(p => p.Line == line)
                .OrderByDescending(p => p.Date)
                .ThenByDescending(p => p.Time)
                .Take(50)
                .ToListAsync();
            // Проходим по каждому элементу и проверяем существование файла
            foreach (var prod in prods)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "pdf", $"{prod.Label}.pdf");
                prod.FileExists = System.IO.File.Exists(filePath);
            }

            // Создаем анонимный объект и передаем в представление
            return View(Tuple.Create(prods.AsEnumerable(), printer));
        }

        // GET: Prod/Details/5
        public async Task<IActionResult> Details(string label)
        {
            if (label == null)
            {
                return NotFound();
            }

            // Форматируем путь к файлу PDF
            string pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "pdf", $"{label}.pdf");

            // Проверяем существование файла
            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound($"Указанного файла {pathToFile} нет на сервере.");
            }

            // Читаем файл и отправляем его обратно клиенту
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathToFile);
            return File(fileBytes, "application/pdf", $"{label}.pdf");
        }

        // GET: Prod/Print
        public async Task<IActionResult> Print(string label, string printer)
        {
            if (label == null)
            {
                return NotFound();
            }

            // Форматируем путь к файлу PDF
            string pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "pdf", $"{label}.pdf");

            // Проверяем существование файла
            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound($"Указанного файла {pathToFile} нет на сервере.");
            }

            // Читаем файл и отправляем его обратно клиенту
            try
            {
                // Код для печати PDF-файла
                System.IO.File.Copy(pathToFile, @"\\NAS\" + printer, true);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка печати {pathToFile} на {printer}: {ex.Message}");
                return NoContent();
            }
        }

    }
}