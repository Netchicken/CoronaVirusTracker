using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirusTracker.Models
{
    public partial class Tracker
    {

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string ASPNetUsersIdfk { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Phone { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateIn { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateOut { get; set; }


    }
}
