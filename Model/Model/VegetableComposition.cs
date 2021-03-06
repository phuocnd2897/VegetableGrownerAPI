﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("VegetableComposition")]
    public class VegetableComposition
    {
        public VegetableComposition()
        {
            Id = Guid.NewGuid().ToString();
            Keywords = new HashSet<Keyword>();
            Labels = new HashSet<Label>();
        }
        [Required, MaxLength(128)]
        public string Id { get; set; }
        [Required, MaxLength(128)]
        public string CompositionName { get; set; }
        public string VegetableDescriptionId { get; set; }
        [ForeignKey("VegetableDescriptionId")]
        public virtual VegetableDescription VegetableDescription { get; set; }
        public virtual ICollection<Keyword> Keywords { get; set; }
        public virtual ICollection<Label> Labels { get; set; }
    }
}
