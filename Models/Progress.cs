using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagement.Models
{
    public class Progress
    {
        // Первичный ключ (идентификатор)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Дата (обязательно)
        [Required]
        public DateOnly Date { get; set; }

        // Смена (может быть NULL)
        [MaxLength(1)]
        public char? Shift { get; set; }

        // Материал (может быть NULL)
        [MaxLength(10)]
        public string Material { get; set; }

        // Описание (может быть NULL)
        [MaxLength(255)]
        public string Description { get; set; }

        // Недостающая партия (может быть NULL)
        public int? Backlog { get; set; }

        // Плановое производство (может быть NULL)
        public int? Planned { get; set; }

        // Фактический результат производства (может быть NULL)
        public int? Result { get; set; }
    }
}