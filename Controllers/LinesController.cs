using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;
using ProductionManagement.Models;

namespace ProductionManagement.Controllers
{
    public class LinesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LinesController(ApplicationDbContext context) => _context = context;

        // GET: Lines
        public async Task<IActionResult> Index() =>
            View(await _context.Lines.ToListAsync());

        // GET: Lines/Details/id?mode=edit/create/delete
        public async Task<IActionResult> Details(string id, string mode)
        {
            if (id == null && mode != "create") return NotFound();

            var line = await _context.Lines.FindAsync(id);
            ViewBag.mode = mode;
            switch (mode)
            {
                case "create": return View(new Line());
                case "edit":
                    if (line == null) return NotFound();
                    return View(line);
                case "delete":
                    if (line == null) return NotFound();
                    return View(line);
                default: return BadRequest();
            }
        }

        // POST: Lines/Details/id?mode=edit/create/delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(Line line, string mode)
        {
            if (!ModelState.IsValid) return View(line);
            // Выводим в консоль все поля переданной модели
            Console.WriteLine($"Полученная модель:");
            Console.WriteLine($"\tName: {line.name}");
            Console.WriteLine($"\tIP: {line.ip}");
            Console.WriteLine($"\tPort: {line.port}");
            Console.WriteLine($"\tPrinter: {line.printer}");
            Console.WriteLine($"\tPrint Label: {line.print_label}");
            Console.WriteLine($"\tIs Online: {line.is_online}");
            Console.WriteLine($"\tLast Check: {line.last_check}");

            switch (mode)
            {
                case "create": _context.Add(line); break;
                case "edit": _context.Update(line); break;
                case "delete": _context.Remove(line); break;
                default: return BadRequest();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
