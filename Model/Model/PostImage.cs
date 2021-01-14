using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("PostImage")]
    public class PostImage
    {
        public PostImage()
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
        public string PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }

    }
}
