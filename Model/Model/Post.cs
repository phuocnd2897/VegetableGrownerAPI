using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("Post")]
    public class Post
    {
        public Post()
        {
            Id = Guid.NewGuid().ToString();
            ExchangeDetails = new HashSet<ExchangeDetail>();
            VegetableExchanges = new HashSet<VegetableExchange>();
        }
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required]
        public string PostContent { get; set; }
        public int Quantity { get; set; }
        public int Type { get; set; }
        public bool Status { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string Address { get; set; }
        public DateTime DateShare { get; set; }
        [Required, MaxLength(128)]
        public string AccountId { get; set; }
        [Required, MaxLength(128)]
        public string VegetableId { get; set; }
        public bool Lock { get; set; }
        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }
        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }
        [ForeignKey("WardId")]
        public virtual Ward Ward { get; set; }
        [ForeignKey("AccountId")]
        public virtual AppAccount AppAccount { get; set; }
        [ForeignKey("VegetableId")]
        public virtual Vegetable Vegetable { get; set; }
        public virtual ICollection<ExchangeDetail> ExchangeDetails { get; set; }
        public virtual ICollection<VegetableExchange> VegetableExchanges { get; set; }
    }
}
