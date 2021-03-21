using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class ExchangeDetailResponseModel
    {
        public string Id { get; set; }
        public string VegName { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FullNameHost { get; set; }
        public string FullNameReceiver { get; set; }
        public string AccountHostId { get; set; }
        public string ReceiverId { get; set; }
        public string ShareDetailId { get; set; }
    }
}
