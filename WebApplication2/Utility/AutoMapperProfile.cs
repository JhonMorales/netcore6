using AutoMapper;
using WebAPIAutores.Entity;
using WebApplication2.DTOs;
using WebApplication2.Entity;

namespace WebApplication2.Utility
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AutorCreateDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            //CreateMap<Autor, AutorDTO>()
            //    .ForMember(autorDTO=> autorDTO.Libros, options=> options.MapFrom(MapLibroDTOLibros));
            CreateMap<Autor, AutorDTOLibro>()
                .ForMember(autorDTO => autorDTO.Libros, options => options.MapFrom(MapLibroDTOLibros));
            CreateMap<LibroCreateDTO, Libro>()
                .ForMember(libro => libro.AutoresLibros, options => options.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOAutor>()
                .ForMember(libroDTO=>libroDTO.Autores, opciones=>opciones.MapFrom(MapLibroDTOAutores));
            CreateMap<LibroPatchDTO, Libro>().ReverseMap();
            CreateMap<ComentarioCreateDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
        }

        private List<LibroDTO> MapLibroDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();
            if (autor.AutoresLibros == null)
            {
                return resultado;
            }
            foreach(var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }

            return resultado;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();
            if (libro.AutoresLibros == null)
            {
                return resultado;
            }
            foreach (var autorlibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autorlibro.AutorId,
                    Name = autorlibro.Autor.Name
                });
            }
            return resultado;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreateDTO libroCreateDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();
            if (libroCreateDTO == null)
            {
                return resultado;
            }

            foreach(var autorId in libroCreateDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }

            return resultado;
        }
    }
}
