using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VG.Model.RequestModel
{
    public class ExchangeDetailRequestModel
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public int QuantityExchange { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        [Required, MaxLength(200)]
        public string Address { get; set; }
        public string FullAddress { get; set; }
        public string PostId { get; set; }
        public string VegetableId { get; set; }
    }
}
