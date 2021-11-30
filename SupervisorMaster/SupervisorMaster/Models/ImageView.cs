using System;
using System.Collections.Generic;
using System.Text;

namespace SupervisorMaster.Models
{
    public class ImageView
    {
        public string Id { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime UploadTime { get; set; }
    }
}
