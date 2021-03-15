using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("ShareDetail")]
    public class ShareDetail
    {
        public ShareDetail()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required]
        public string ShareContent { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public DateTime DateShare { get; set; }
        [Required, MaxLength(128)]
        public string AccountId { get; set; }
        [Required, MaxLength(128)]
        public string VegetableId { get; set; }
        [ForeignKey("AccountId")]
        public virtual AppAccount AppAccount { get; set; }
        [ForeignKey("VegetableId")]
        public virtual Vegetable Vegetable { get; set; }
    }
}
