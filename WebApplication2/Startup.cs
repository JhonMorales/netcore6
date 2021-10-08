using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
//using WebAPIAutores.Services;
using WebApplication2.Filters;
using WebApplication2.Middlewares;
using WebApplication2.Services;
//using WebApplication2.Services;

namespace WebAPIAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(ExceptionFilter));
            }).AddJsonOptions(
                x=>x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
            //services.AddDbContext<ApplicationDbContext>(options => options.UseOracle(Configuration.GetConnectionString("defaultConnection")));
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")
                ));
            //services.AddTransient<IService, ServicioA>(); //Mantiene la data temporalemnte mientras se ejecuta la función
            //services.AddTransient<ServicioA>();
            //services.AddScoped<IService, ServicioA>(); //Mantiene en memoria la data por cada peticion realizada e individualemente.
            //services.AddTransient<IService, ServicioA>(); //Comparte la data en memoria con todas las peticiones realizadas, tiempo no importa

            //services.AddTransient<ServicioTransient>();
            //services.AddScoped<ServicioScoped>();
            //services.AddSingleton<ServicioSingleton>();
            //services.AddTransient<MiFiltroDeAccion>();
            //services.AddHostedService<WriteFile>();

            //services.AddResponseCaching();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["keyJWT"])),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIAutores", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                { 
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
            });
            services.AddDataProtection();
            services.AddTransient<HashService>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://www.apirequest.io").AllowAnyMethod().AllowAnyHeader();
                    //.WithExposedHeaders();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            //app.UseMiddleware<LogearRespuestaMiddleware>();
            app.UseLogearRespuestaHTTP();

            //bifurcación es decir crea otro camino en la tuberia solo para esta ruta en específico
            //app.Map("/ruta1", app =>
            //{
            //    app.Run(async contexto =>
            //    {
            //        await contexto.Response.WriteAsync("middleware, interceptando tuberia");
            //    });
            //});

            //intercepta la tuberia y impide la ejecución de los demás middleware
            //app.Run(async contexto =>
            //{
            //    await contexto.Response.WriteAsync("middleware, interceptando tuberia");
            //});
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAutores v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            //app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
