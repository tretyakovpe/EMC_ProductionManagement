using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagement.Models
{
    public class Line
    {
        // Свойство для поля 'name' в базе данных
        [Key] // Указываем, что это первичный ключ
        [Column(TypeName = "nchar(3)")] // Тип данных и длина
        [Required] // Обязательное свойство
        public string name { get; set; }

        // Свойство для поля 'ip'
        [Column(TypeName = "nchar(15)")]
        [Required]
        public string ip { get; set; }

        // Свойство для поля 'port'
        [Column(TypeName = "int")]
        public int? port { get; set; } // nullable, так как значение может быть null

        // Свойство для поля 'printer'
        [Column(TypeName = "nchar(10)")]
        public string printer { get; set; } // Nullable, так как значение может быть null

        // Свойство для поля 'print_label'
        [Column(TypeName = "bit")]
        [Required]
        public bool print_label { get; set; }

        // Свойство для поля 'is_online'
        [Column(TypeName = "bit")]
        [Required]
        public bool is_online { get; set; }

        // Свойство для поля 'last_check'
        [Column(TypeName = "datetime")]
        public DateTime? last_check { get; set; } // Nullable, так как значение может быть null
    }
}
