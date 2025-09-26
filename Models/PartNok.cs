using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagement.Models
{
    public class PartNok
    {
        // Primary key (счетчик)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        // Название детали (ограничено 10 символами)
        [Required]
        [Column("name", TypeName = "nchar(10)")]
        public string Name { get; set; }

        // Дата и время
        [Required]
        [Column("datetime")]
        public DateTime Datetime { get; set; }

        // Счетчик (целое число)
        [Required]
        [Column("counter", TypeName="int")]
        public int Counter { get; set; }

        // MKM (4 байта)
        [Required]
        [Column("mkm", TypeName = "varbinary(4)")]
        public byte[] Mkm { get; set; }

        //Название видеофайла
        [Column("video", TypeName = "nvarchar(50)")]
        public string Video { get; set; }

        //Номер линии
        [Column("line", TypeName = "nvarchar(3)")]
        public string Line { get; set; }

        //свойство FileExists, которое не сохраняется в базу данных. Нужно для проверки на контроллере
        [NotMapped]
        public bool FileExists { get; set; }

    }
}