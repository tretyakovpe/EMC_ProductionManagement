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
        public async Task<IActionResult> Index()
        {
            var result = await _context.Lines
                                      .Select(l => new Line
                                      {
                                          Name = l.Name ?? "",
                                          Ip = l.Ip ?? "",
                                          Port = l.Port ?? 0,
                                          Printer = l.Printer ?? "",
                                          PrintLabel = l.PrintLabel,
                                          IsOnline = l.IsOnline,
                                          LastCheck = l.LastCheck,
                                          IsActive = l.IsActive,
                                          Camera = l.Camera ?? "no"
                                      })
                                      .ToListAsync();

            return View(result);
        }
        //Получение полного списка линий.
        public async Task<IActionResult> GetAllLines()
        {
            var result = await _context.Lines
                                       .Select(l => new Line
                                       {
                                           Name = l.Name ?? "",
                                           Ip = l.Ip ?? "",
                                           Port = l.Port ?? 0,
                                           Printer = l.Printer ?? "",
                                           PrintLabel = l.PrintLabel,
                                           IsOnline = l.IsOnline,
                                           LastCheck = l.LastCheck,
                                           IsActive = l.IsActive,
                                           Camera = l.Camera ?? " "
                                       })
                                       .OrderBy(l => l.Name)
                                       .ToListAsync();

            return Json(result);
        }

        // GET: Lines/Details/id?mode=edit/create/delete
        public async Task<IActionResult> Details(string id, string mode)
        {
            if (id == null && mode != "create") return NotFound();
            var line = await _context.Lines.FindAsync(id);
            ViewBag.Mode = mode;
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
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Ошибка проверки модели: {error.ErrorMessage}");
                    }
                }
                return View(line);
            }

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

        // GET: Lines/Delete/5
        public IActionResult Delete(int id)
        {
            // Получаем запись по ID
            var line = _context.Lines.Find(id);

            if (line == null)
            {
                return NotFound(); // Если запись не найдена
            }

            // Устанавливаем режим удаления
            ViewBag.Mode = "delete";

            return View("Details", line); // Возвращаем представление Details с установленным режимом удаления
        }

        // POST: Lines/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Получаем запись по ID
            var line = await _context.Lines.FindAsync(id);

            if (line != null)
            {
                _context.Lines.Remove(line);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // Перенаправляем на список записей после успешного удаления
        }
    }
}
