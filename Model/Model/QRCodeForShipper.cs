using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("QRCodeForShipper")]
    public class QRCodeForShipper
    {
        public QRCodeForShipper()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required, MaxLength(128)]
        public string Name { get; set; }
        [Required, MaxLength(200)]
        public string Url { get; set; }
        [Required, MaxLength(128)]
        public string ExchangeId { get; set; }
        [ForeignKey("ExchangeId")]
        public virtual ExchangeDetail ExchangeDetail { get; set; }
    }
}
