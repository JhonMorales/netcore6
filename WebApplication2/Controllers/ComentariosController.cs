using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores;
using WebApplication2.DTOs;
using WebApplication2.Entity;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [HttpGet(Name = "obtenerComentariosLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existsLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existsLibro)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios.Where(comentarioDB => comentarioDB.LibroId == libroId).ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name ="obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetId(int id)
        {
            var existsComentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);
            if (existsComentario == null)
            {
                return BadRequest("No existe el comentario solicitado");
            }
            return mapper.Map<ComentarioDTO>(existsComentario);

            
        }

        [HttpPost(Name = "crearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCreateDTO comentarioCreateDTO)
        {
            var claimEmail = HttpContext.User.Claims.Where(claim=> claim.Type == "email").FirstOrDefault();
            var email = claimEmail.Value;
            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var existsLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existsLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreateDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = userId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            //return Ok();
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("getComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarComentario")]
        public async Task<ActionResult> Put(ComentarioCreateDTO comentarioCreateDTO, int id, int libroId) 
        { 
            var existsLibro = await context.Libros.AnyAsync(libroDB=> libroDB.Id == libroId);
            if (!existsLibro) { return NotFound(); }

            var existsComentario = await context.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id);
            if (!existsComentario) {  return NotFound(); }

            var comentario = mapper.Map<Comentario>(comentarioCreateDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();
        }


    }
}
