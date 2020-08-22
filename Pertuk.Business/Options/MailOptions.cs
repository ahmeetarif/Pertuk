using System;
using System.Collections.Generic;
using System.Text;

namespace Pertuk.Business.Options
{
    public class MailOptions
    {
        public string BaseUrl { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
    }
}
