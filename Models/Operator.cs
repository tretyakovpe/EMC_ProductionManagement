using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagement.Models;
public class Operator
{
    // Автоинкрементируемый первичный ключ
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // Поле ФИО оператора длиной максимум 255 символов
    [Required]
    [Column("name", TypeName = "nchar(255)")] // Тип данных и длина
    [StringLength(255)]
    public string Name { get; set; }
}
