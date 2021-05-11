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
        public string FullName { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string Address { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime ExpiresTime { get; set; }
        public string Token { get; set; }
        public string DeviceToken { get; set; }
        public int RoleId { get; set; }
    }
}
