using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("Report")]
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string ReportContent { get; set; }
        public DateTime DateReport { get; set; }
        [Required, MaxLength(128)]
        public string ShareDetailId { get; set; }
        [Required, MaxLength(128)]
        public string AccountReport { get; set; }
        [ForeignKey("ShareDetailId")]
        public virtual ShareDetail ShareDetail { get; set; }
        [ForeignKey("AccountReport")]
        public virtual AppAccount AppAccount { get; set; }
    }
}
