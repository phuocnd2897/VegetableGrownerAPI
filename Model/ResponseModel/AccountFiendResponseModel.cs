using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class AccountFiendResponseModel
    {
        public int Id { get; set; }
        public string AccountSend { get; set; }
        public string FriendName { get; set; }
        public DateTime RequestedDate { get; set; }
    }
}
