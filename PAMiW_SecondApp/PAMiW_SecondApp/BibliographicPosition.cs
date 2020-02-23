using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMiW_SecondApp
{
    [Serializable]
    public class BibliographicPosition
    {
        public string Guid { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<File> Files { get; set; }
        public List<Link> Links { get; set; }
    }
}
