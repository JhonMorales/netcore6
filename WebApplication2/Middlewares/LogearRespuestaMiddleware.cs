namespace WebApplication2.Middlewares
{
    public static class LogearRespuestaHTTPMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogearRespuestaMiddleware>();
        }
    }

    public class LogearRespuestaMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LogearRespuestaMiddleware> logger;

        public LogearRespuestaMiddleware(RequestDelegate siguiente, ILogger<LogearRespuestaMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext contexto) {
            using (var ms = new MemoryStream())
            {
                var cuerpoRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;
                await siguiente(contexto);
                ms.Seek(0, SeekOrigin.Begin);

                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(cuerpoRespuesta);
                contexto.Response.Body = cuerpoRespuesta;

                logger.LogInformation(respuesta);
            }
        }
    }
}
