using System;
using System.Collections.Generic;
using System.Text;

namespace SupervisorMaster.Models
{
    public class WebsiteView : BaseXamarin.Models.BaseModel
    {
        private string url;

        public string Url { get => url; set => SetProperty(ref url, value); }
    }
}
