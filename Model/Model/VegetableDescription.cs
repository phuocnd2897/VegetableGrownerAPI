using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("VegetableDescription")]
    public class VegetableDescription
    {
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required, MaxLength(128)]
        public string VegContent { get; set; }
        public int VegCompositionId { get; set; }
        public string VegetableId { get; set; }
        [ForeignKey("VegCompositionId")]
        public virtual VegetableComposition VegetableComposition { get; set; }
        [ForeignKey("VegetableId")]
        public virtual Vegetable Vegetable { get; set; }
    }
}
