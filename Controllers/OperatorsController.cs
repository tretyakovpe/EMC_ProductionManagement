using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;
using ProductionManagement.Models;

namespace ProductionManagement.Controllers
{
    public class OperatorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OperatorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(GetAllOperators());
        }

        // Получаем список операторов
        [HttpGet]
        public async Task<JsonResult> GetAllOperators()
        {
            var operators = await _context.Operators.ToListAsync();
            return Json(operators);
        }

        // Метод для добавления нового оператора
        [HttpPost]
        public async Task<ActionResult<string>> Add(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return BadRequest("Имя не заполнено");

            var newOperator = new Operator { Name = fullName.Trim() };
            _context.Operators.Add(newOperator);
            await _context.SaveChangesAsync();
            return Ok($"Запись успешно создана.");
        }

        // Метод для обновления оператора
        [HttpPut]
        public async Task<ActionResult<string>> Update(int id, string fullName)
        {
            var existingOperator = await _context.Operators.FirstOrDefaultAsync(x => x.Id == id);
            if (existingOperator == null) return NotFound("Оператора не существует");

            existingOperator.Name = fullName.Trim();
            await _context.SaveChangesAsync();
            return Ok($"Запись {id} успешно обновлена на {fullName}.");
        }

        // Метод для удаления оператора
        [HttpDelete]
        public async Task<ActionResult<string>> Delete(int id)
        {
            var existingOperator = await _context.Operators.FirstOrDefaultAsync(x => x.Id == id);
            if (existingOperator == null) return NotFound("Оператора не существует");

            _context.Operators.Remove(existingOperator);
            await _context.SaveChangesAsync();
            return Ok($"Запись {existingOperator.Name} успешно удалена.");
        }
    }
}