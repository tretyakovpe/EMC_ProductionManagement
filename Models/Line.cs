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

    //Значение счётчика, для интерфейса.
    [NotMapped]
    public int Counter { get; set; }

    //// Переопределение Equals для сравнения значимых свойств
    //public override bool Equals(object obj)
    //{
    //    if (obj is Line other)
    //    {
    //        return Name == other.Name &&
    //               Ip == other.Ip &&
    //               Port == other.Port &&
    //               Printer == other.Printer &&
    //               PrintLabel == other.PrintLabel &&
    //               IsOnline == other.IsOnline &&
    //               LastCheck == other.LastCheck &&
    //               IsActive == other.IsActive;
    //    }
    //    return false;
    //}

    // Переопределение GetHashCode для корректной работы с коллекциями
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Ip, Port, Printer, PrintLabel, IsOnline, LastCheck, IsActive);
    }
}