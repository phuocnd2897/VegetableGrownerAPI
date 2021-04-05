using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class WikiResponseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Feature { get; set; }
        public IEnumerable<string> ListText { get; set; }

    }
}
