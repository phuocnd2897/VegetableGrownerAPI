﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("VegetableDescription")]
    public class VegetableDescription
    {
        public VegetableDescription()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required, MaxLength(1000)]
        public string VegContent { get; set; }
        public int VegetableCompositionId { get; set; }
        public string AccountId { get; set; }
        [ForeignKey("VegetableCompositionId")]
        public virtual VegetableComposition VegetableComposition { get; set; }
        [ForeignKey("AccountId")]
        public virtual AppAccount AppAccount { get; set; }

    }
}
