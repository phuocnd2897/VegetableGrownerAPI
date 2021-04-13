using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("VegetableShare")]
    public class VegetableShare
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string VegetableDesciptionId { get; set; }
        public string ShareDetailId { get; set; }
        [ForeignKey("ShareDetailId")]
        public virtual ShareDetail ShareDetail { get; set; }
        [ForeignKey("VegetableDesciptionId")]
        public virtual VegetableDescription VegetableDescription { get; set; }
    }
}
