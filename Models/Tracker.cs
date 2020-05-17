using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirusTracker.Models
{
    [BindProperties] //to tell model binding to target all public properties of the class:(SupportsGet = true)
    public partial class Tracker
    {

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ASPNetUsersIdfk { get; set; }


        public string BusinessName { get; set; }
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

        /*   [FromQuery(Name = "Place")]  //populates it from the query */
        public string Place { get; set; }

    }
}
