using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class PostResponseModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string PostContent { get; set; }
        public string Description { get; set; }
        public string Feature { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AccountId { get; set; }
        public IEnumerable<string> listImage { get; set; }
    }
}
