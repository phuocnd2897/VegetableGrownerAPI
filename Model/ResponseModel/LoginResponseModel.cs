using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class LoginResponseModel
    {
        public string AccountId { get; set; }
        public string ProviderKey { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime ExpiresTime { get; set; }
        public string Token { get; set; }
        public int RoleId { get; set; }
    }
}
