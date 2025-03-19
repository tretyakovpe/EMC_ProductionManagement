using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagement.Models
{
    public class Material
    {
        // Свойство для поля 'MaterialID' в базе данных
        [Key] // Указываем, что это первичный ключ
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент
        public int MaterialID { get; set; }

        // Свойство для поля 'MaterialCode'
        [Column(TypeName = "nchar(10)")]
        [Required] // Обязательное свойство
        public string MaterialCode { get; set; }

        // Свойство для поля 'CustomerCode'
        [Column(TypeName = "nchar(10)")]
        [Required]
        public string CustomerCode { get; set; }

        // Свойство для поля 'Destination'
        [Column(TypeName = "nchar(10)")]
        [Required]
        public string Destination { get; set; }

        // Свойство для поля 'HU'
        [Column(TypeName = "nchar(10)")]
        [Required]
        public string HU { get; set; }

        // Свойство для поля 'Netto'
        [Column(TypeName = "int")]
        [Required]
        public int Netto { get; set; }

        // Свойство для поля 'Brutto'
        [Column(TypeName = "int")]
        [Required]
        public int Brutto { get; set; }

        // Свойство для поля 'QuantityInHU'
        [Column(TypeName = "int")]
        [Required]
        public int QuantityInHU { get; set; }
    }
}
