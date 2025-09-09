using System.ComponentModel.DataAnnotations;

namespace ProductionManagement.Models
{
    public class Shift
    {
        // Первичный ключ (Auto-increment)
        public int Id { get; set; }

        // Номер производственной линии
        [Required]
        public string LineName { get; set; }

        // Уникальный идентификатор оператора
        [Required]
        public int OperatorId { get; set; }

        // Время начала смены
        [Required]
        public DateTime StartTime { get; set; }
    }
}