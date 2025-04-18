﻿using System.ComponentModel.DataAnnotations;

namespace ProductionManagement.Models
{
    public class Prod
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}")]
        public TimeSpan Time { get; set; }

        [Key]
        [Required]
        [StringLength(12)]
        public string Label { get; set; }

        [Required]
        [StringLength(3)]
        public string Line { get; set; }

        [Required]
        [StringLength(10)]
        public string Material { get; set; }

        [Required]
        public int Amount { get; set; }
    }
}