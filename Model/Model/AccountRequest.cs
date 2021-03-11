using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("AccountRequest")]
    public class AccountRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(128)]
        public string AccountSend { get; set; }
        [Required, MaxLength(128)]
        public string AccountReceived { get; set; }
        public DateTime RequestDate { get; set; }
        public int Status { get; set; }
        [ForeignKey("AccountSend")]
        public virtual AppAccount AppAccount { get; set; }
    }
}
