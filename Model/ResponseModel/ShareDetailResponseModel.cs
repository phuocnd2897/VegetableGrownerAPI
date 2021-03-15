using System;
using System.Collections.Generic;
using System.Text;
using VG.Model.Model;

namespace VG.Model.ResponseModel
{
    public class ShareDetailResponseModel
    {
        public string Id { get; set; }
        public string VegName { get; set; }
        public string VegDescription { get; set; }
        public string VegFeature { get; set; }
        public int Quantity { get; set; }
        public int Statius { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AccountId { get; set; }
        public IEnumerable<VegetableImage> Images { get; set; }
    }
}
