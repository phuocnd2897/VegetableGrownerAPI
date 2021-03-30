using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("ExchangeDetail")]
    public class ExchangeDetail
    {
        public ExchangeDetail()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        public DateTime DateExchange { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        [Required, MaxLength(128)]
        public string Sender { get; set; }
        [Required, MaxLength(128)]
        public string ReceiveBy { get; set; }
        [Required, MaxLength(128)]
        public string ShareDetailId { get; set; }
        public string VegetableId { get; set; }
        [ForeignKey("ReceiveBy")]
        public virtual AppAccount AppAccount { get; set; }
        [ForeignKey("ShareDetailId")]
        public virtual ShareDetail ShareDetail { get; set; }
        [ForeignKey("VegetableId")]
        public virtual Vegetable Vegetable { get; set; }
    }
}
