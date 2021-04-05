using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VG.Model.Model
{
    [Table("AppAccountLogin")]
    public class AppAccountLogin
    {
        public AppAccountLogin()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string ProviderKey { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime ExpiresTime { get; set; }
        public string DeviceToken { get; set; }
        public string AppAccountId { get; set; }
        [ForeignKey("AppAccountId")]
        public virtual AppAccount AppAccount { get; set; }
    }
}
