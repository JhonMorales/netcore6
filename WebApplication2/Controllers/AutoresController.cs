using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Entity;
using WebAPIAutores.Services;
using WebApplication2.DTOs;
using WebApplication2.Filters;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    //[Route("/api/[controller]")]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "esAdmin")]
    //[Authorize]
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        //private readonly IService service;
        //private readonly ServicioSingleton servicioSingleton;
        //private readonly ServicioScoped servicioScoped;
        //private readonly ServicioTransient servicioTransient;
        //private readonly ILogger<AutoresController> logger;

        //public AutoresController(ApplicationDbContext context, IService service, ServicioSingleton
        //    servicioSingleton, ServicioScoped servicioScoped, ServicioTransient servicioTransient, ILogger<AutoresController> logger)
        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            //this.service = service;
            //this.servicioSingleton = servicioSingleton;
            //this.servicioScoped = servicioScoped;
            //this.servicioTransient = servicioTransient;
            //this.logger = logger;
        }

        //[HttpGet("configuraciones")]
        //public ActionResult<string> obtenerConfiguracion()
        //{
        //    return configuration["connectionStrings:defaultConnection"];
        //}

        [HttpGet(Name = "obtenerAutores")]
        //[HttpGet("listado")]
        //[HttpGet("/listado")]
        //[ServiceFilter(typeof(MiFiltroDeAccion))]
        //[Authorize]
        [AllowAnonymous]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            //throw new NotImplementedException();
            //logger.LogInformation("Obteniendo Autores");
            //logger.LogWarning("Mensaje de Prueba"); //Obteniendo de nivel en appsettings
            //return await context.Autores.Include(x=>x.Libros).ToListAsync();
            var autores =  await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        //[HttpGet("primero")]
        //public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
        //{
        //    return await context.Autores.FirstOrDefaultAsync();
        //}

        //[HttpGet("{id:int}/{param2?}")]
        //[HttpGet("{id:int}/{param2=persona}")]
        //public async Task<ActionResult<Autor>> Get([FromRoute] int id, string nombre) {
        [HttpGet("{id:int}", Name = "obtenerAutor")]
        //public async Task<ActionResult<AutorDTO>> Get([FromRoute] int id)
        public async Task<ActionResult<AutorDTOLibro>> Get([FromRoute] int id)
        {
            var getAutorId = await context.Autores
                .Include(autorDB=> autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(autorBD => autorBD.Id == id);
            if (getAutorId == null)
            {
                return NotFound();
            }
            //return mapper.Map<AutorDTO>(getAutorId);
            return mapper.Map<AutorDTOLibro>(getAutorId);
        }

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> Get(String nombre)
        {
            //var getAutorId = await context.Autores.FirstOrDefaultAsync(autorBD => autorBD.Name.Contains(nombre));
            var autores = await context.Autores.Where(autorBD => autorBD.Name.Contains(nombre)).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        //[HttpGet("GUID")]
        ////[ResponseCache(Duration =10)]
        //[ServiceFilter(typeof(MiFiltroDeAccion))]
        //public ActionResult Guid()
        //{
        //    return Ok(new {
        //        AutoresControllerTransient = servicioTransient.Guid,
        //        ServicioA_Transient = service.ObtenerTransient(),
        //        AutoresControllerSingleton = servicioSingleton.Guid,
        //        ServicioA_Singleton = service.ObtenerSingleton(),
        //        AutoresControllerScoped = servicioScoped.Guid,
        //        ServicioA_Scoped = service.ObtenerScoped(),
        //    });
        //}

        [HttpPost(Name = "crearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorCreateDTO autorCreateDTO)
        {
            var existsAutor = await context.Autores.AnyAsync(x => x.Name == autorCreateDTO.Name);
            if (existsAutor) {
                return BadRequest("El nombre ya existe"); 
            }

            //var autor = new Autor()
            //{
            //    Name = autorCreateDTO.Name
            //};

            var autor = mapper.Map<Autor>(autorCreateDTO);

            context.Add(autor);
            await context.SaveChangesAsync();
            //return Ok();
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutor", new { id= autor.Id}, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutor")]
        public async Task<ActionResult> Put(AutorCreateDTO autorCreateDTO, int id)
        {
            //if (autor.Id != id)
            //{
            //    return BadRequest("El id del autor no corresponde con el de la URL");
            //}

            var exists = await context.Autores.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreateDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            //return Ok();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name ="borrarAutor")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Autores.AnyAsync(x => x.Id == id);

            if (!exists) {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        private void GenerarEnlaces(AutorDTO autorDTO)
        {
            autorDTO.Enlaces.Add(
                new DatoHATEOAS(
                    enlace: Url.Link(
                        "obtenerAutor", 
                        new {id = autorDTO.Id}
                        ),
                    descripcion: "self",
                    metodo: "GET"
                    )
                );
        }
    }
}
