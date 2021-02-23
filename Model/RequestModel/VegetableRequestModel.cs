using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VG.Model.RequestModel
{
    public class VegetableRequestModel
    {
        [Required, MaxLength(50)]
        public string Title { get; set; }
        [Required, MaxLength(200)]
        public string Description { get; set; }
        [Required, MaxLength(200)]
        public string Featture { get; set; }
        public IEnumerable<IFormFile> Images { get; set; }
        public int GardenId { get; set; }
    }
}
