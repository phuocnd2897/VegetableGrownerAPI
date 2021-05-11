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
        public int Stt { get; set; }
        public DateTime DateExchange { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        [MaxLength(128)]
        public string Sender { get; set; }
        [Required, MaxLength(128)]
        public string ReceiveBy { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string Address { get; set; }
        [Required, MaxLength(128)]
        public string PostId { get; set; }
        public string VegetableId { get; set; }
        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }
        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }
        [ForeignKey("WardId")]
        public virtual Ward Ward { get; set; }
        [ForeignKey("ReceiveBy")]
        public virtual AppAccount AppAccountReceive { get; set; }
        [ForeignKey("Sender")]
        public virtual AppAccount AppAccountSender { get; set; }
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
        [ForeignKey("VegetableId")]
        public virtual Vegetable Vegetable { get; set; }
    }
}
