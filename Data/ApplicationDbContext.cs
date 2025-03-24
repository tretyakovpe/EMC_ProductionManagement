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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка соответствия модели Line с таблицей PLC
            modelBuilder.Entity<Line>().ToTable("plc");

            // Настройка ключей и отношений для таблицы PLC
            modelBuilder.Entity<Line>().HasKey(l => l.Name); // Предположим, что Name является ключевым полем

            // Настройка соответствия модели Material с таблицей Materials
            modelBuilder.Entity<Material>().ToTable("materials");

            // Настройка ключей и отношений для таблицы PLC
            modelBuilder.Entity<Material>().HasKey(m => m.MaterialID); // Предположим, что Name является ключевым полем

            modelBuilder.Entity<Prod>().ToTable("prod");
        }
    }
}
