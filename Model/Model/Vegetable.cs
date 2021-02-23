using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("Vegetable")]
    public class Vegetable
    {
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required, MaxLength(128)]
        public string VegName { get; set; }
        [Required, MaxLength(128)]
        public int GardenId { get; set; }
        [ForeignKey("GardenId")]
        public virtual Garden Garden { get; set; } 
    }
}
