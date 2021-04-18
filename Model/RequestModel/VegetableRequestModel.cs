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
        public string Id { get; set; }
        [Required, MaxLength(50)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Featture { get; set; }
        public int? Quantity { get; set; }
        public string Images { get; set; }
        public IEnumerable<IFormFile>? NewImages { get; set; }
        public int GardenId { get; set; }
        public string IdDescription { get; set; }
        public bool IsFixed { get; set; }
        public string NameSearch { get; set; }
        public string SynonymOfFeature { get; set; } 
    }
}
