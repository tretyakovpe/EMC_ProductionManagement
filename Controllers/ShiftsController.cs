using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;
using ProductionManagement.Models;

namespace ProductionManagement.Controllers
{
    public class ShiftsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShiftsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Список всех смен
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Shifts.ToListAsync());
        }

        // Получение всех смен (API)
        [HttpGet]
        public async Task<JsonResult> GetAllShifts()
        {
            var shifts = await _context.Shifts.Include(s => s.OperatorId).ToListAsync();
            return Json(shifts);
        }

        // Добавление новой смены
        [HttpPost]
        public async Task<IActionResult> AddShift(Shift newShift)
        {
            if (ModelState.IsValid)
            {
                _context.Shifts.Add(newShift);
                await _context.SaveChangesAsync();
                return Ok("Новая смена успешно добавлена.");
            }
            return BadRequest("Ошибка при добавлении смены.");
        }

        // Обновление смены
        [HttpPut]
        public async Task<IActionResult> UpdateShift(int id, Shift updatedShift)
        {
            var existingShift = await _context.Shifts.FindAsync(id);
            if (existingShift == null) return NotFound("Смены не найдено.");

            existingShift.LineName = updatedShift.LineName;
            existingShift.OperatorId = updatedShift.OperatorId;
            existingShift.StartTime = updatedShift.StartTime;

            await _context.SaveChangesAsync();
            return Ok("Смена успешно обновлена.");
        }

        // Удаление смены
        [HttpDelete]
        public async Task<IActionResult> DeleteShift(int id)
        {
            var shiftToRemove = await _context.Shifts.FindAsync(id);
            if (shiftToRemove == null) return NotFound("Смены не найдено.");

            _context.Shifts.Remove(shiftToRemove);
            await _context.SaveChangesAsync();
            return Ok("Смена успешно удалена.");
        }
    }
}