using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("PrecentReport")]
    public class PrecentReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Precent { get; set; }
    }
}
