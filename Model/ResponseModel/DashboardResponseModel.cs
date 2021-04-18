using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class DashboardResponseModel
    {
        public string[] labels { get; set; }
        public List<int[]> series { get; set; }
    }
}
