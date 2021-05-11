using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace VG.Model.RequestModel
{
    public class KeywordsRequestModel
    {
        public string text { get; set; }
        public IEnumerable<Dictionary<string, string>> db { get; set; }
    }
}
