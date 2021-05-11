using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("Garden")]
    public class Garden
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(500)]
        public string Name { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        [Required, MaxLength(500)]
        public string Address { get; set; }
        [Required, MaxLength(128)]
        public string AccountId { get; set; }
        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }
        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }
        [ForeignKey("WardId")]
        public virtual Ward Ward { get; set; }
        [ForeignKey("AccountId")]
        public virtual AppAccount AppAccount { get; set; }

    }
}
