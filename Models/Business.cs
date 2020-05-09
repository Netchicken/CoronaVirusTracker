using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirusTracker.Models
{
    public partial class Business
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string BusinessName { get; set; }
        [Required]
        [StringLength(50)]
        public string AdminName { get; set; }
        [Required]
        [StringLength(50)]
        public string BusinessAddress { get; set; }
        [Required]
        [StringLength(50)]
        public string Phone { get; set; }
    }
}
