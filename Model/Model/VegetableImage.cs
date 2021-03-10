using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("PostImage")]
    public class VegetableImage
    {
        public VegetableImage()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; }
        [Required, MaxLength(200)]
        public string Thumbnail { get; set; }
        [Required, MaxLength(200)]
        public string Url { get; set; }
        [Required, MaxLength(128)]
        public string LocalUrl { get; set; }
        [Required, MaxLength(128)]
        public string VegetableDescriptionId { get; set; }
        [ForeignKey("VegetableDescriptionId")]
        public virtual VegetableDescription VegetableDescription { get; set; }

    }
}
