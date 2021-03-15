using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.RequestModel
{
    public class ExchangeDetailRequestModel
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public string ShareDetailId { get; set; }
        public string ReceivedBy { get; set; }
    }
}
