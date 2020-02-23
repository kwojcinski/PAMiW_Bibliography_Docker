using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAMiW_REST
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Passwd { get; set; }
        public List<string> Positions { get; set; }
        public List<string> Files { get; set; }
        public bool Logged { get; set; }
    }
}
