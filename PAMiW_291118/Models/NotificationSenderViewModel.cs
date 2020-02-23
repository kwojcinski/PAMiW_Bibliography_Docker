using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSETutorial.Models
{
    public class NotificationsSenderViewModel
    {
        public bool Alert { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public string Notification { get; set; }
    }
}
