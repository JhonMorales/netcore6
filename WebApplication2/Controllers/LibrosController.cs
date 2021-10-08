using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Entity;
using WebApplication2.DTOs;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper) {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "obtenerLibros")]
        public async Task<ActionResult<List<LibroDTO>>> Index() {
            //var libros = await context.Libros.Include(libroBD => libroBD.Comentarios).ToListAsync();
            var libros = await context.Libros.ToListAsync();
            return mapper.Map<List<LibroDTO>>(libros);
        }

        [HttpGet("{id:int}", Name = "obtenerLibro")]
        //public async Task<ActionResult<LibroDTO>> Get(int id) {
        public async Task<ActionResult<LibroDTOAutor>> Get(int id)
        {
            //return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
            //return await context.Libros.FirstOrDefaultAsync(x => x.Id == id);
            //var getLibro = await context.Libros.Include(libroBD => libroBD.Comentarios).FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            var getLibro = await context.Libros
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            if (getLibro == null) { return NotFound("El libro no existe"); }

            getLibro.AutoresLibros = getLibro.AutoresLibros.OrderBy(x=> x.Orden).ToList();
            return mapper.Map<LibroDTOAutor>(getLibro);
        }

        [HttpPost(Name = "crearLibro")]
        public async Task<ActionResult> Post(LibroCreateDTO libroCreateDTO)
        {
            //var existAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            //if (!existAutor)
            //{
            //    return BadRequest($"No existe el autor id: {libro.AutorId}");
            //}

            if (libroCreateDTO.AutoresIds == null) { return BadRequest("No se puede crear un libro sin autores"); }

            var autoresIds = await context.Autores.Where(autorBD => libroCreateDTO.AutoresIds.Contains(autorBD.Id))
                .Select(x=>x.Id).ToListAsync();

            if (libroCreateDTO.AutoresIds.Count !=  autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreateDTO);

            AssignOrderAutors(libro);

            context.Add(libro);
            await context.SaveChangesAsync();
            //return Ok();
            var libroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);

        }

        [HttpPut("{id:int}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreateDTO libroCreateDTO)
        {
            var libroDB = await context.Libros.
                Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null) { return NotFound(); }

            AssignOrderAutors(libroDB);

            libroDB = mapper.Map(libroCreateDTO, libroDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(JsonPatchDocument<LibroPatchDTO> patchDocument, int id) 
        { 
            if (patchDocument== null) {  return BadRequest(); }

            var libroDB = await context.Libros.FirstOrDefaultAsync(x =>  x.Id == id);
            if (libroDB == null) {  return NotFound(); }

            var libroDTO = mapper.Map<LibroPatchDTO>(libroDB);
            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);
            if (!esValido) { return BadRequest(ModelState); }

            mapper.Map(libroDTO, libroDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Libros.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        private void AssignOrderAutors(Libro libro)
        {
            for (int i = 0; i < libro.AutoresLibros.Count; i++)
            {
                libro.AutoresLibros[i].Orden = i;
            }
        }
    }
}
