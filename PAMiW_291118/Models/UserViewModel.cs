using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAMiW_291118.Models
{
    public class UserViewModel : User
    {
        public List<PDF> PDFs { get; set; }
    }
}
