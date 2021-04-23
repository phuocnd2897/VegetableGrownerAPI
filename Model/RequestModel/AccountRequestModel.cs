using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VG.Model.RequestModel
{
    public class AccountRequestModel
    {
        public string Id { get; set; }
        [Required, MaxLength(10)]
        public string PhoneNumber { get; set; }
        [Required, MinLength(6), MaxLength(128)]
        public string Password { get; set; }
        [Required, MaxLength(200)]
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Sex { get; set; }
        public string Address { get; set; }
        [Required, MaxLength(500)]
        public string Email { get; set; }
        public IFormFile? AvatarRequest { get; set; }
        public string AvatarResponse { get; set; }
        public int IsFriend { get; set; }
    }
}
