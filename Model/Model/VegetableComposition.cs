using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("VegetableComposition")]
    public class VegetableComposition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(128)]
        public string CompositionName { get; set; }
        public string VegetableDescriptionId { get; set; }
        [ForeignKey("VegetableDescriptionId")]
        public virtual VegetableDescription VegetableDescription { get; set; }
    }
}
