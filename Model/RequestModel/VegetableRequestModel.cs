using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VG.Model.Model;

namespace VG.Model.RequestModel
{
    public class VegetableRequestModel
    {
        public string IdName { get; set; }
        public string IdDescription { get; set; }
        public string IdFeature { get; set; }
        public string IdImage { get; set; }
        [Required, MaxLength(50)]
        public string Title { get; set; }
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        public string Featture { get; set; }
        public string NewFeatture { get; set; }
        public int? Quantity { get; set; }
        public IEnumerable<VegetableImage>? Images { get; set; }
        public IEnumerable<IFormFile>? NewImages { get; set; }
        public int GardenId { get; set; }
        public string IdDetailName { get; set; }
        public string IdDetailDescription { get; set; }
        public string IdDetailFeature { get; set; }
        public string IdDetailImage { get; set; }
    }
}
