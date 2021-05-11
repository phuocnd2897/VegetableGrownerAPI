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
        public Vegetable()
        {
            Id = Guid.NewGuid().ToString();
            Posts = new HashSet<Post>();
        }
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        public int No { get; set; }
        public int? Quantity { get; set; }
        public int GardenId { get; set; }
        [Required, MaxLength(128)]
        public string VegetableDescriptionId { get; set; }
        [ForeignKey("GardenId")]
        public virtual Garden Garden { get; set; }
        [ForeignKey("VegetableDescriptionId")]
        public virtual VegetableDescription VegetableDescription { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
