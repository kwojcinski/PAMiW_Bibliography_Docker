using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PAMiW_291118.Models
{
    public class BibliographicPosition
    {
        public string Guid { get; set; }
        [Required(ErrorMessage = "Podaj tytuł.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Podaj autora.")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Podaj datę publikacji.")]
        public DateTime PublicationDate { get; set; }
        public List<File> Files { get; set; }
        public List<Link> Links { get; set; }
        public string UserId { get; set; }
    }
}
