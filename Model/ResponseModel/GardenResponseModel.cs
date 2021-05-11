using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class GardenResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int WardId { get; set; }
        public string WardName { get; set; }
        public string Address { get; set; }
    }
}
