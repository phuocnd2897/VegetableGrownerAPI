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
        [Key]
        [Required, MaxLength(128)]
        public string ReceiveBy { get; set; }
        [Key]
        [Required, MaxLength(128)]
        public string AccountId { get; set; }
        [Key]
        [Required, MaxLength(128)]
        public string VegetableId { get; set; }
        public virtual AppAccount AppAccount { get; set; }
        public virtual Vegetable Vegetable { get; set; }
    }
}
