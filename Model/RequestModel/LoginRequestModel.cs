using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VG.Model.RequestModel
{
    public class LoginRequestModel
    {
        [Required, MaxLength(10)]
        public string PhoneNumber { get; set; }
        [Required, MaxLength(128)]
        public string Password { get; set; }
        public string DeviceToken { get; set; }
    }
}
