using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;
using ProductionManagement.Models;

namespace ProductionManagement.Services
{
    public class PartsManager
    {
        private readonly ApplicationDbContext _context;

        public PartsManager(ApplicationDbContext context)
        {
            _context = context;
        }

        // Добавление новой записи PartNok в базу данных
        public async Task AddPartNokAsync(PartNok partNok)
        {
            await _context.partsNok.AddAsync(partNok);
            await _context.SaveChangesAsync();
        }

        // Получение списка всех записей PartNok
        public async Task<List<PartNok>> GetAllPartNoksAsync()
        {
            return await _context.partsNok.ToListAsync();
        }

        // Получение последних 50 записей PartNok
        public async Task<List<PartNok>> GetLatest50PartNoksAsync()
        {
            return await _context.partsNok
                .OrderByDescending(p => p.Datetime)
                .Take(50)
                .ToListAsync();
        }

        // Получение конкретной записи PartNok по идентификатору
        public async Task<PartNok?> GetPartNokByIdAsync(int id)
        {
            return await _context.partsNok.FindAsync(id);
        }
    }
}