using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("Account")]
    public class Account
    {
        public Account()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        [MaxLength(128)]
        public string Id { get; set; }
        [Required, MaxLength(10)]
        public string PhoneNumber { get; set; }
        [Required, MinLength(6), MaxLength(128)]
        public string PassWord { get; set; }
        public bool Lock { get; set; }
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
