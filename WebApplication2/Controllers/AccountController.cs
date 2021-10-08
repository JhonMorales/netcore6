using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication2.DTOs;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public AccountController(
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration, 
            SignInManager<IdentityUser> signInManager, 
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico_y_quizas_secreto");
        }

        [HttpPost("signup", Name = "crearUsario")] //api/accounts/signup
        public async Task<ActionResult<AuthenticateResponseDTO>> Resgister(UserCredentialDTO userCredentialDTO)
        {
            var user = new IdentityUser { UserName = userCredentialDTO.Email, Email = userCredentialDTO.Email };
            var resultado = await userManager.CreateAsync(user, userCredentialDTO.Password);

            if (resultado.Succeeded)
            {
                return await CreateToken(userCredentialDTO);
            }
            else 
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login", Name = "loginUsuario")]
        public async Task<ActionResult<AuthenticateResponseDTO>> Login(UserCredentialDTO userCredentialDTO)
        {
            var result = await signInManager.PasswordSignInAsync(
                userCredentialDTO.Email, userCredentialDTO.Password, isPersistent: false, lockoutOnFailure: false
                );

            if (result.Succeeded)
            {
                return await CreateToken(userCredentialDTO);
            }else
            {
                return BadRequest("Login Incorrecto");
            }
        }
        [HttpGet("RenovarToken", Name = "renovarToken")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticateResponseDTO>> Renovar() 
        {
            var claimEmail = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = claimEmail.Value;
            var credenciales = new UserCredentialDTO()
            {
                Email = email
            };
            return await CreateToken(credenciales);
        }

        private async Task<AuthenticateResponseDTO> CreateToken(UserCredentialDTO userCredentialDTO) 
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentialDTO.Email),
                new Claim("lo que quiera", "Cualquier valor")
            };

            var usuario = await userManager.FindByEmailAsync(userCredentialDTO.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);
            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyJWT"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var expired = DateTime.UtcNow.AddYears(1);
            var expired = DateTime.UtcNow.AddMinutes(30);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expired, signingCredentials: creds);

            return new AuthenticateResponseDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                ExpirateAt = expired
            };
        }

        [HttpPost("CreateAdmin", Name ="crearAdmin")]
        public async Task<ActionResult> EditAdmin(EditAdminDTO editAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));

            return NoContent();
        }

        [HttpPost("RemoveAdmin", Name = "removerAdmin")]
        public async Task<ActionResult> RemoveAdmin(EditAdminDTO editAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));

            return NoContent();
        }

        //[HttpGet("hash/{textoPlano}")]
        //public ActionResult RealizarHash(string textoPlano)
        //{
        //    var resultado1 = hashService.Hash(textoPlano);
        //    var resultado2 = hashService.Hash(textoPlano);

        //    return Ok(new
        //    {
        //        textoPlano = textoPlano,
        //        hash1 = resultado1,
        //        hash2 = resultado2
        //    });
        //}

        //[HttpGet("Encriptar")]
        //public ActionResult Encriptar()
        //{
        //    var textoPlano = "prueba";
        //    var textoCifrado = dataProtector.Protect(textoPlano);
        //    var textoDesencriptado = dataProtector.Unprotect(textoCifrado);

        //    return Ok(new
        //    {
        //        textoPlano = textoPlano,
        //        textoEncriptado = textoCifrado,
        //        textoDesencriptado = textoDesencriptado
        //    });
        //}

        //[HttpGet("EncriptarPorTiempo")]
        //public ActionResult EncriptarPorTiempo()
        //{
        //    var protectorLimitadoPorTiempo = dataProtector.ToTimeLimitedDataProtector();

        //    var textoPlano = "prueba";
        //    var textoCifrado = protectorLimitadoPorTiempo.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5));
        //    Thread.Sleep(6000);
        //    var textoDesencriptado = protectorLimitadoPorTiempo.Unprotect(textoCifrado);

        //    return Ok(new
        //    {
        //        textoPlano = textoPlano,
        //        textoEncriptado = textoCifrado,
        //        textoDesencriptado = textoDesencriptado
        //    });
        //}
    }
}
