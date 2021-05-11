using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("Label")]
    public class Label
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string LabelName { get; set; }
        public string StandsFor { get; set; }
        public string VegCompositionId { get; set; }
        [ForeignKey("VegCompositionId")]
        public virtual VegetableComposition VegetableComposition { get; set; }
    }
}
