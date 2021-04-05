using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class AccountRequestResponseModel
    {
        public int Id { get; set; }
        public string AccountSend { get; set; }
        public string AccountSendName { get; set; }
        public string AccountReceived { get; set; }
        public string AccountReceivedName { get; set; }
        public DateTime RequestedDate { get; set; }
        public int Status { get; set; }
    }
}
