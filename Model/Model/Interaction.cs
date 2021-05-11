using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    public class Interaction
    {
        public Interaction()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        [Required, MaxLength(128)]
        public string AccountId { get; set; }
        [Required, MaxLength(128)]
        public string PostId { get; set; }
        [ForeignKey("AccountId")]
        public virtual AppAccount AppAccount { get; set; }
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }

    }
}
