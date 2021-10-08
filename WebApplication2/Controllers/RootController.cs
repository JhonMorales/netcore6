using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController: ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            
            datosHateoas.Add(new DatoHATEOAS(
                enlace: Url.Link("ObtenerRoot", new {}),
                descripcion: "self",
                metodo: "GET"));

            datosHateoas.Add(new DatoHATEOAS(
                enlace: Url.Link("obtenerAutores", new {}),
                descripcion: "autores",
                metodo: "GET"));

            //datosHateoas.Add(new DatoHATEOAS(
            //    enlace: Url.Link("obtenerAutor", new {}),
            //    descripcion: "autor",
            //    metodo: "GET"));

            //datosHateoas.Add(new DatoHATEOAS(
            //    enlace: Url.Link("obtenerAutorPorNombre", new {}),
            //    descripcion: "autor por nombre",
            //    metodo: "GET"));

            datosHateoas.Add(new DatoHATEOAS(
                enlace: Url.Link("obtenerLibros", new {}),
                descripcion: "libros",
                metodo: "GET"));

            //datosHateoas.Add(new DatoHATEOAS(
            //    enlace: Url.Link("obtenerLibro", new {}),
            //    descripcion: "obtener libro",
            //    metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOAS(
                    enlace: Url.Link("crearAutor", new { }),
                    descripcion: "crear-autor",
                    metodo: "POST"));

                datosHateoas.Add(new DatoHATEOAS(
                   enlace: Url.Link("crearLibro", new { }),
                   descripcion: "crear-libro",
                   metodo: "POST"));
            }
           
            
            return datosHateoas;
        }
    }
}
