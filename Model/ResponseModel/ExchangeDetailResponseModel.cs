using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class ExchangeDetailResponseModel
    {
        public string Id { get; set; }
        public string VegNameReceive { get; set; }
        public int Quantity { get; set; }
        public string VegNameReceiveExchangeResponse { get; set; }
        public int QuantityReceiveExchangeResponse { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FullNameHost { get; set; }
        public string FullNameReceiver { get; set; }
        public string AccountHostId { get; set; }
        public string ReceiverId { get; set; }
        public string PostId { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string QRCode { get; set; }
        public int TypeShare { get; set; }
    }
}
