using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace VirusTracker.Models
{
    public partial class Business
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ASPNetUsersIdfk { get; set; }
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
