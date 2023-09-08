using API.Auxiliares;
using API.DTOs;
using API.DTOs.OtrosSistemas;
using API.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Servicios;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;
        private readonly IMetodosAUX _metodosAUX;
        //private readonly IEmpleadosServicio _empleadosServicio;

        public CuentasController(UserManager<IdentityUser> userManager, 
                                 IConfiguration configuration, 
                                 SignInManager<IdentityUser> signInManager, 
                                 IDataProtectionProvider dataProtectionProvider,
                                 HashService hashService,
                                 IMetodosAUX metodosAUX)
                                 //IEmpleadosServicio empleadosServicio)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("CA3MWMYAJdaW4TH61Th95kuVMSU51n9iA");
            this._metodosAUX = metodosAUX;
            //this._empleadosServicio = empleadosServicio;
        }


        [HttpGet("hash/{textoPlano}")]
        public ActionResult RealizarHash(string textoPlano)
        {

            var resultado1 = hashService.Hash(textoPlano); 
            var resultado2= hashService.Hash(textoPlano);

            return Ok(new
            {
                textoPlano = textoPlano,
                Hash1 = resultado1,
                Hash2 = resultado2
            });
        }


        [HttpGet("Cifrar")]
        public ActionResult Cifrar() {
            var textoplano = "oscar cantero";
            var textocifrado=dataProtector.Protect(textoplano);
            var textodescifrado = dataProtector.Unprotect(textocifrado);

            return Ok(new
            {
                textoplano = textoplano,
                textocifrado = textocifrado,
                textodescifrado = textodescifrado
            });
        }

        [HttpGet("CifrarPorTiempo")]
        public ActionResult CifrarPorTiempo()
        {

            var protectorLimitadoPorTiempo = dataProtector.ToTimeLimitedDataProtector();

            var textoplano = "oscar cantero";
            var textocifrado = protectorLimitadoPorTiempo.Protect(textoplano,lifetime:TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var textodescifrado = protectorLimitadoPorTiempo.Unprotect(textocifrado);

            return Ok(new
            {
                textoplano = textoplano,
                textocifrado = textocifrado,
                textodescifrado = textodescifrado
            });
        }
        [HttpPost("registrar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }

        }

        [HttpPost("login")]

        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            try
            {
                var usuarioMail = userManager.FindByEmailAsync(credencialesUsuario.Email).Result;
                var usuarioUser = userManager.FindByNameAsync(credencialesUsuario.Email).Result;

                var user = usuarioMail ?? usuarioUser;

                if(user==null)
                    return Ok("Login Incorrecto*****pass");

                var resultado = await signInManager.PasswordSignInAsync(user.UserName, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);


                if (resultado.Succeeded)
                {
                    return await ConstruirToken(credencialesUsuario);
                }
                else
                {
                    return Ok("Login Incorrecto*****pass");
                }
            }catch(Exception ex) 
            {
                return BadRequest("Login Incorrecto*****"+ex.Message);
            }
           


        }


        [HttpPost("GetTocken")]
        public async Task<ActionResult<RespuestaAutenticacion>> GetTocken(CredencialesUsuario credencialesUsuario)
        {
            var user = userManager.FindByNameAsync(credencialesUsuario.Email).Result;

            if (user == null)
                return Ok("Login Incorrecto*****pass");

            var resultado = await signInManager.PasswordSignInAsync(user.UserName, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);


            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return Ok("Login Incorrecto*****pass");
            }
        }


        [HttpGet("RenovarToken")]  
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public  async Task<ActionResult<RespuestaAutenticacion>> Renovar(string email)
        {
            var emailClaim0 = HttpContext.User.Claims;
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            //var email = emailClaim.Value;

            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = emailClaim.Value
            };
            return  await ConstruirToken(credencialesUsuario);
        }
        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
           
            var usuarioMail = userManager.FindByEmailAsync(credencialesUsuario.Email).Result;
            var usuarioName = userManager.FindByNameAsync(credencialesUsuario.Email).Result;
            var usuario = usuarioName ?? usuarioMail;
            var claimsDB = await userManager.GetClaimsAsync(usuario);
            var Roles = await userManager.GetRolesAsync(usuario);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role,Roles.First()),
                new Claim(ClaimTypes.NameIdentifier,usuario.Id),
                new Claim("Id",usuario.Id),
                new Claim("email",credencialesUsuario.Email),
                new Claim("Role",Roles.First())
            };

            var keyPlease = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));

            claims.AddRange(claimsDB);
            var llave = keyPlease; //new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var cred = new SigningCredentials(llave, SecurityAlgorithms.HmacSha512);
            var expiracion = DateTime.UtcNow.AddMinutes(35);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: cred);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion,
                EmpleadoId = usuario.Id,
                RolId = Roles.First()
            };
        }
        [HttpPost("HacerAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Policy = "EsAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        
        }

        [HttpPost("RemoverAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

        [HttpGet("PasswordOlvidado")]
        public async Task<JsonResult> PasswordOlvidado(string usuario)
        {
            var usuarioMail = userManager.FindByEmailAsync(usuario).Result;
            var usuarioUser = userManager.FindByNameAsync(usuario).Result;
            var user = usuarioMail ?? usuarioUser;

            if (user == null)
                return new JsonResult(new { status = "Usuario no encontrado" }); 
            else
            {
                var newPass = user.UserName + DateTime.Now.Ticks.ToString();
                userManager.RemovePasswordAsync(user);
                userManager.AddPasswordAsync(user, newPass);

                _metodosAUX.MandarCorreoPass(user.UserName, user.Email, newPass);
            }

            return new JsonResult(new { status = Constantes.RESPUESTA_OK });
        }

        [HttpGet("CambiarPassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<JsonResult> CambiarPassword(string usuarioId,string usuarioCorreo ,string actualPass,string newPass)
        {
            var usuarioIddBase = userManager.FindByIdAsync(usuarioId).Result;
            var usuarioCorreoBase = userManager.FindByIdAsync(usuarioCorreo).Result;

            var user = usuarioIddBase ?? usuarioCorreoBase;

            if (usuarioIddBase != usuarioCorreoBase)
                return new JsonResult(new { status = "Por favor verifica tu correo" });

            if (user == null)
                return new JsonResult(new { status = "usuario no encontrado" });
            else
            {
                var cambio = userManager.ChangePasswordAsync(user, actualPass, newPass).Result;
                if (cambio.Succeeded)
                {
                    _metodosAUX.MandarCorreoPass(user.UserName, user.Email, newPass);
                    return new JsonResult(new { status = Constantes.RESPUESTA_OK });
                }
                else
                {
                    return new JsonResult(new { status = "Por favor verifica tus datos" });
                }

            }
        }


        [HttpGet]
        public IActionResult Get()
        {
            if (HttpContext.Request.Headers.TryGetValue("Origin", out var origin))
            {
                if (HttpContext.Request.HttpContext.Response.Headers.TryGetValue("Access-Control-Allow-Origin", out var allowOrigin))
                {
                    // CORS está habilitado y la solicitud tiene acceso permitido desde el dominio especificado
                    // Puedes utilizar el valor de allowOrigin para tomar decisiones basadas en el dominio permitido
                    return Ok("Solicitud permitida por CORS");
                }
                else
                {
                    // CORS está habilitado pero la solicitud no tiene acceso permitido desde el dominio
                    return BadRequest("Solicitud denegada por CORS");
                }
            }
            else
            {
                // La solicitud no es una solicitud CORS
                return Ok("No es una solicitud CORS");
            }
        }




    }
}
