using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAMiW_REST
{
    public class FileViewModel : File
    {
        public string UserGuid { get; set; }
        public IFormFile PDF { get; set; }
        public string PDFbytes { get; set; }
    }
}
