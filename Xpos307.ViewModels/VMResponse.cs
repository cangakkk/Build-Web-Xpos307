using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Xpos307.ViewModels
{
    public class VMResponse
    {
        public bool Success { get; set; }
        public string message { get; set; }
        public object entity { get; set; }

        public VMResponse()
        {
            Success = true;
        }
    }
}
