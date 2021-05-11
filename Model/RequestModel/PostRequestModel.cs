using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VG.Model.RequestModel
{
    public class PostRequestModel
    {
        public string Id { get; set; }
        [Required, MaxLength(1000)]
        public string Content { get; set; }
        public int Quantity { get; set; }
        public int Type { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        [Required, MaxLength(200)]
        public string Address { get; set; }
        public string VegetableId  { get; set; }
        public IEnumerable<string> VegetableNeedId { get; set; }
        
    }
}
