using System;
using System.Collections.Generic;
using System.Text;
using VG.Model.Model;

namespace VG.Model.ResponseModel
{
    public class PostResponseModel
    {
        public string Id { get; set; }
        public string VegetableId { get; set; }
        public string VegName { get; set; }
        public string VegDescription { get; set; }
        public string VegFeature { get; set; }
        public int Quantity { get; set; }
        public int QuantityVeg { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
        public IEnumerable<VegetableExchangeResponseModel> VegetableExchange { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public IEnumerable<VegetableImage> Images { get; set; }
    }
}
