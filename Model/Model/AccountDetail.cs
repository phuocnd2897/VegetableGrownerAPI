using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("AccountDetail")]
    public class AccountDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Sex { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Required, MaxLength(50)]
        public string AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
    }
}
