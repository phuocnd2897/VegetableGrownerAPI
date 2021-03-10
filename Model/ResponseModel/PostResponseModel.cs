using System;
using System.Collections.Generic;
using System.Text;
using VG.Model.Model;

namespace VG.Model.ResponseModel
{
    public class PostResponseModel
    {
        public string Id { get; set; }
        public string VegName { get; set; }
        public string PostContent { get; set; }
        public string VegDescription { get; set; }
        public string VegFeature { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AccountId { get; set; }
        public IEnumerable<VegetableImage> Images { get; set; }
    }
}
