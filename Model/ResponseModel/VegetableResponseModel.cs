﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using VG.Model.Model;

namespace VG.Model.ResponseModel
{
    public class VegetableResponseModel
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Feature { get; set; }
        public int? Quantity { get; set; }
        public IEnumerable<VegetableImage> Images { get;set; }
        public int GardenId { get; set; }
        public string IdName { get; set; }
        public string IdDescription { get; set; }
        public string IdFeature { get; set; }
        public string IdImage { get; set; }
        public string IdDetailName { get; set; }
        public string IdDetailDescription { get; set; }
        public string IdDetailFeature { get; set; }
        public string IdDetailImage { get; set; }
    }
}
