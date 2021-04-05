using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("AppAccount")]
    public class AppAccount
    {
        public AppAccount()
        {
            Id = Guid.NewGuid().ToString();
            Members = new HashSet<Member>();
            Gardens = new HashSet<Garden>();
            AppAccountLogins = new HashSet<AppAccountLogin>();
        }
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required, MaxLength(10)]
        public string PhoneNumber { get; set; }
        [Required, MinLength(6), MaxLength(128)]
        public string PassWord { get; set; }
        public bool Status { get; set; }
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual AccountRole Role { get; set; }
        public virtual ICollection<AppAccountLogin> AppAccountLogins { get; set; }
        public virtual ICollection<Member> Members { get; set; }
        public virtual ICollection<Garden> Gardens { get; set; }
    }
}
