using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirusTracker.Models
{
    public partial class Tracker
    {

        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column("BusinessIDFK")]
        public Guid BusinessIdfk { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Phone { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateIn { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateOut { get; set; }

        [ForeignKey(nameof(BusinessIdfk))]
        public virtual Business BusinessIdfkNavigation { get; set; }
    }
}
