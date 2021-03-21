using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("KeyWord")]
    public class Keyword
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string KeywordName { get; set; }
        public int VegCompositionId { get; set; }
        public string AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual AppAccount AppAccount { get; set; }
        [ForeignKey("VegCompositionId")]
        public virtual VegetableComposition VegetableComposition { get; set; }
    }
}
