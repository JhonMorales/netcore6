using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPIAutores.Validations;
using WebApplication2.Entity;

namespace WebAPIAutores.Entity
{
    //public class Autor: IValidatableObject
    public class Autor
    {
        public int Id{ get; set; }
        
        
        //[Range(18,120)]
        //[NotMapped] //no migra la columna al modelo solo es para probar
        //public int Edad {  get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:120, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]
        [FirstLetterUppercase]
        public String Name{ get; set; }
        public List<AutorLibro> AutoresLibros {  get; set; }
        //[CreditCard]
        //[NotMapped]
        //public String tarjetaCredito { get; set; }
        //[Url]
        //[NotMapped]
        //public String url { get; set; }
        //[NotMapped]
        //public int menor {  get; set; }
        //[NotMapped]
        //public int mayor { get; set; }
        //public List<Libro> Libros { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        //    if (!string.IsNullOrEmpty(Name)) { 
        //        var primeraLetra = Name.ToString()[0].ToString();

        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser mayúscula", 
        //                new string[] { nameof(Name)});
        //        }
        //    }
        //    //if (menor > mayor)
        //    //{
        //    //    yield return new ValidationResult("Este valor no puede ser mas grande que el campo Mayor", 
        //    //        new string[] { nameof(menor)});
        //    //}
        //}
    }
}
