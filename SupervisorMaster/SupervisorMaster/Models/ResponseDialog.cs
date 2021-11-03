using System;
using System.Collections.Generic;
using System.Text;

namespace SupervisorMaster.Models
{
    public class ResponseDialog
    {
        public bool Result { get; set; }
        public object[] Objs { get; set; }
    }

    public class Errors
    {
        public string domain { get; set; }
        public string reason { get; set; }
        public string message { get; set; }
    }

    public class Error
    {
        public List<Error> errors { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class ResponseFirebase
    {
        public Error error { get; set; }
    }
}
