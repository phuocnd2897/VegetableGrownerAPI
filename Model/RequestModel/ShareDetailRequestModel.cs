using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VG.Model.RequestModel
{
    public class ShareDetailRequestModel
    {
        public string Id { get; set; }
        [Required, MaxLength(1000)]
        public string Content { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public string VegetableId  { get; set; }
    }
}
