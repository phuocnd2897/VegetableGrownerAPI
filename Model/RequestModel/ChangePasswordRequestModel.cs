using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.RequestModel
{
    public class ChangePasswordRequestModel
    {
        public string PhoneNumber { get; set; }
        public string NewPassword { get; set; }
    }
}
