using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagement.Models;

public class Line
{
    // Свойство для поля 'name' в базе данных
    [Key] // Указываем, что это первичный ключ
    [Column("name", TypeName = "nchar(3)")] // Тип данных и длина
    [Required] // Обязательное свойство
    public string Name { get; set; }

    // Свойство для поля 'ip'
    [Column("ip", TypeName = "nchar(15)")]
    [Required]
    public string Ip { get; set; }

    // Свойство для поля 'port'
    [Column("port", TypeName = "int")]
    public int? Port { get; set; } // nullable, так как значение может быть null

    // Свойство для поля 'printer'
    [Column("printer", TypeName = "nchar(10)")]
    public string Printer { get; set; } // Nullable, так как значение может быть null

    // Свойство для поля 'print_label'
    [Column("print_label", TypeName = "bit")]
    [Required]
    public bool PrintLabel { get; set; }

    // Свойство для поля 'is_online'
    [Column("is_online", TypeName = "bit")]
    public bool IsOnline { get; set; }

    // Свойство для поля 'last_check'
    [Column("last_check", TypeName = "datetime")]
    public DateTime? LastCheck { get; set; } // Nullable, так как значение может быть null

    // Свойство для поля 'is_active'
    [Column("is_active", TypeName = "bit")]
    public bool IsActive { get; set; }

    // Свойство для поля 'camera'
    [Column("camera", TypeName = "varchar(255)")]
    public string Camera { get; set; } //Nullable

    //Значение счётчика, для интерфейса.
    [NotMapped]
    public int Counter { get; set; }

    // Переопределение GetHashCode для корректной работы с коллекциями
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Ip, Port, Printer, PrintLabel, IsOnline, LastCheck, IsActive);
    }
}