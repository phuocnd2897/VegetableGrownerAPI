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
        [Required, MaxLength(200)]
        public string Tittle { get; set; }
        [Required, MaxLength(1000)]
        public string Content { get; set; }
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        [Required, MaxLength(1000)]
        public string Feature { get; set; }
        public IEnumerable<IFormFile> Images { get; set; }
    }
}
