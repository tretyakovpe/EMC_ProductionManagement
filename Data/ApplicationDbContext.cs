using Microsoft.EntityFrameworkCore;
using ProductionManagement.Models;

namespace ProductionManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Набор данных для таблицы PLC (название модели остается Line)
        public DbSet<Line> Lines { get; set; }

        public DbSet<Material> Materials { get; set; }

        public DbSet<Prod> Prods { get; set; }

        public DbSet<Operator> Operators { get; set; }

        public DbSet<Shift> Shifts { get; set; }

        public DbSet<PartNok> partsNok { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка соответствия модели Line с таблицей PLC
            modelBuilder.Entity<Line>().ToTable("plc");

            // Настройка ключей и отношений для таблицы PLC
            modelBuilder.Entity<Line>().HasKey(l => l.Name); // Предположим, что Name является ключевым полем

            // Настройка соответствия модели Material с таблицей Materials
            modelBuilder.Entity<Material>().ToTable("materials");

            // Настройка ключей и отношений для таблицы Materials
            modelBuilder.Entity<Material>().HasKey(m => m.MaterialID); // Предположим, что Name является ключевым полем

            // Настройка соответствия модели Operator с таблицей Operators
            modelBuilder.Entity<Operator>().ToTable("operators");

            // Настройка ключей и отношений для таблицы Operators
            modelBuilder.Entity<Operator>().HasKey(o => o.Id);

            // Настройка соответствия модели Shift с таблицей Shifts
            modelBuilder.Entity<Shift>().ToTable("shifts");

            // Настройка ключей и отношений для таблицы Shifts
            modelBuilder.Entity<Shift>().HasKey(o => o.Id);

            // Настройка соответствия модели PartNok с таблицей partNok
            modelBuilder.Entity<PartNok>().ToTable("partNok");

            // Настройка ключей и отношений для таблицы partNok
            modelBuilder.Entity<PartNok>().HasKey(o => o.Id);

            modelBuilder.Entity<Prod>().ToTable("prod");
        }
    }
}
