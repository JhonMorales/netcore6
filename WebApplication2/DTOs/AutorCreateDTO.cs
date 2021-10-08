using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validations;

namespace WebApplication2.DTOs
{
    public class AutorCreateDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]
        [FirstLetterUppercase]
        public string Name { get; set; }
    }
}
