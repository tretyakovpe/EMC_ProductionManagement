using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagement.Models
{
    public class Prod
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}")]
        public TimeSpan time { get; set; }

        [Key]
        [Required]
        [StringLength(12)]
        public string label { get; set; }

        [Required]
        [StringLength(3)]
        public string line { get; set; }

        [Required]
        [StringLength(10)]
        public string material { get; set; }

        [Required]
        public int amount { get; set; }
    }
}