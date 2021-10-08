using WebAPIAutores.Entity;

namespace WebApplication2.Entity
{
    public class AutorLibro
    {
        //public int Id { get; set; }
        public int AutorId { get; set; }
        public int LibroId { get; set; }
        public int Orden {  get; set; }
        public string Name {  get; set; }

        public Libro Libro {  get; set; }
        public Autor Autor {  get; set; }
    }
}
