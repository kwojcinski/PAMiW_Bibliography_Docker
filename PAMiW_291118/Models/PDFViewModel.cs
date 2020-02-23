using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PAMiW_291118.Models
{
    public class PDFViewModel
    {
        public string Login { get; set; }
        [Required(ErrorMessage = "Podaj nazwę pliku.")]
        [MaxLength(120, ErrorMessage = "Zbyt długa nazwa pliku.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Plik PDF jest wymagany.")]
        public IFormFile File { get; set; }
    }
}
