using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAMiW_REST
{
    [Serializable]
    public class File
    {
        public string Guid { get; set; }
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public List<Link> Links { get; set; }
    }
}
