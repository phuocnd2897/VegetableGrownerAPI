using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("AccountFriend")]
    public class AccountFriend
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(128)]
        public string Account_one_Id { get; set; }
        [Required, MaxLength(128)]
        public string Account_two_Id { get; set; }
        public DateTime AcceptedDate { get; set; }
        [ForeignKey("Account_two_Id")]
        public virtual AppAccount AppAccount { get; set; }
    }
}
