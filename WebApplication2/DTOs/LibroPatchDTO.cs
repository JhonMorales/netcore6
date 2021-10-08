using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validations;

namespace WebApplication2.DTOs
{
    public class LibroPatchDTO
    {
        [FirstLetterUppercase]
        [StringLength(maximumLength: 250)]
        [Required]
        public String Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
