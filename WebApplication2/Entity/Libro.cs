using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validations;
using WebApplication2.Entity;

namespace WebAPIAutores.Entity
{
    public class Libro
    {
        public int Id { get; set; }
        [FirstLetterUppercase]
        [Required]
        [StringLength(maximumLength: 250)]
        public String Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public List<Comentario> Comentarios {  get; set; }
        public List<AutorLibro> AutoresLibros {  get; set; }
        //public int AutorId { get; set; }
        //public Autor Autor { get; set; }
    }
}
