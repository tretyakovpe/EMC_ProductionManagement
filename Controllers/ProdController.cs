﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;

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
        public async Task<IActionResult> Index(string line)
        {
            var prods = await _context.Prods
                .Where(p => p.line == line)
                .OrderByDescending(p => p.date)     // Первичная сортировка по дате в порядке убывания
                .ThenByDescending(p => p.time)      // Дополнительная сортировка по времени в порядке убывания
                .ToListAsync();

            return View(prods);
        }

        // GET: Last50
        public async Task<IActionResult> Last50(string line)
        {
            var prods = await _context.Prods
                .Where(p => p.line == line)  // Фильтрация по линии
                .OrderByDescending(p => p.date)  // Сортировка по дате в порядке убывания
                .ThenByDescending(p => p.time)      // Дополнительная сортировка по времени в порядке убывания
                .Take(50)  // Ограничение до 50 последних строк
                .ToListAsync();

            return View(prods);
        }

        // GET: Prod/Details/5
        public async Task<IActionResult> Details(string label)
        {
            if (label == null)
            {
                return NotFound();
            }

            var prod = await _context.Prods
                .FirstOrDefaultAsync(m => m.label == label);
            if (prod == null)
            {
                return NotFound();
            }

            return View(prod);
        }
    }
}