using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("Post")]
    public class Post
    {
        public Post()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required, MaxLength(1000)]
        public string PostContent { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int NoVeg { get; set; }
        public int GardenId { get; set; }
        public string AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual AppAccount Account { get; set; }
    }
}
