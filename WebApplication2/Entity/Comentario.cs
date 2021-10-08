using Microsoft.AspNetCore.Identity;
using WebAPIAutores.Entity;

namespace WebApplication2.Entity
{
    public class Comentario
    {
        public int Id { get; set; }
        public String Contenido { get; set; }
        public int LibroId {  get; set; }
        public Libro Libro {  get; set; }
        public string UsuarioId { get; set; }
        public IdentityUser User {  get; set; }
    }
}
