using API.Auxiliares;
using API.DTOs;
//using API.DTOs.OtrosSistemas;
using AutoMapper;
using Data;
using Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Servicios;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ApiBase.Auxiliares;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmpleadoController : Controller
    {
        private readonly IEmpleadosServicio _empleadosServicio;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration configuration;
        private readonly IMetodosAUX _metodosAUX;
        private readonly IMapper mapper;
        private readonly IHistorialEmpleadoServicio _historialEmpleadoServicio;
        private readonly IContextoPeticion _contexto;

        public EmpleadoController(  ApiConectaContext context, 
                                    IMapper mapper, 
                                    UserManager<IdentityUser> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    IEmpleadosServicio empleadosServicio,
                                    IHistorialEmpleadoServicio historialEmpleadoServicio,
                                    ICatalogAUX catalogAUX,
                                    IMetodosAUX metodosAUX,
                                    IConfiguration configuration,
                                    IHttpContextAccessor httpContextAccessor,
                                    IContextoPeticion contexto)
        {
            this.roleManager = roleManager;
            this._userManager = userManager;
            this._empleadosServicio = empleadosServicio;
            this.configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
            this._metodosAUX = metodosAUX;
            this.mapper = mapper;
            this._historialEmpleadoServicio = historialEmpleadoServicio;
            this._contexto = contexto;
        }





        /// <summary>
        /// Obtiene el rol de un empleado dado su Id
        /// busca el rol del empleado, de no tener rol regresa el rol como un string vacio
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("ObtenerEmpleadoPorId")]
        //[Authorize(Roles = "RHEmpresa, RecursosHumanos, Usuario, Gerente")]
        public async Task<ActionResult<EmpleadoDTO>> ObtenerEmpleadoPorId(int Id)
        {
            try
            {

                var empresaActual = _httpContextAccessor.HttpContext.User.Claims;

                EmpleadoDTO result = new EmpleadoDTO()
                {
                    Nombre ="Silvia",
                    ApellidoPaterno= "Ramirez"
                };

                var user = _userManager.Users.Where(z => z.Id == result.Id.ToString());
                if (user.Any())
                {
                    var roles = await _userManager.GetRolesAsync(user.FirstOrDefault());
                    result.RolId = roles[0];
                    result.userName = user.First().UserName;
                }
                else
                {
                    result.RolId = string.Empty;
                }


                return result;
            }
            catch (Exception ex)
            {
                var debug = ex;
            }
            return NoContent();

        }








        #region Metodos auxiliares para controlador





        /// <summary>
        /// Metodo para crear el usuario de un empleado
        /// </summary>
        /// <param name="empleado"></param>
        /// <param name="nombreRol"></param>
        /// <returns></returns>
        public async Task<string> CrearUsuarioRol(Empleado empleado, string nombreRol,string Usuario)
        {
            string result = string.Empty;

            try
            {
                //var userNameAUX = empleado.CorreoEmpresarial.Split('@')[0];
                IdentityUser user = new IdentityUser()
                {
                    Id = empleado.Id.ToString(),
                    UserName = Usuario,
                    NormalizedUserName = Usuario.ToUpper(),
                    Email = empleado.CorreoEmpresarial,
                    NormalizedEmail = empleado.CorreoEmpresarial.ToUpper(),
                    //PhoneNumber = empleado.,
                };

                string rolClaim = string.Empty;
                string rolName = string.Empty;
                switch (nombreRol)
                {
                    case "Admin":
                        rolClaim = "esAdmin";
                        break;
                    case "Operador":
                        rolClaim = "esOp";
                        break;
                    case "Usuario":
                        rolClaim = "esUs";
                        break;
                }


                _userManager.CreateAsync(user, "n3wPa$$w0rdT3st").Wait();
                _userManager.AddClaimAsync(user, new Claim(rolClaim, "1")).Wait();
                _userManager.AddToRoleAsync(user, nombreRol).Wait();

                result = Constantes.RESPUESTA_OK;

            }
            catch (Exception e)
            {
                result = e.ToString();
            }

            return result;
        }


        /// <summary>
        /// Metodo para crear el historial de empleado, se debe de registrar cuando se cambia el Puesto, Area, Empresa, 
        /// Rol del empleado, salario o bono (y se debe de generar un registro por cada cambio)
        /// </summary>
        /// <param name="empleadoId"></param>
        /// <param name="NuevoCambio"></param>
        /// <param name="anterior"></param>
        /// <param name="identificador"></param>
        /// <param name="_contextEmpleadoId"></param>
        /// <returns></returns>
        public string CrearHistorialEmpleado(int empleadoId, string NuevoCambio, string anterior, string identificador, string _contextEmpleadoId)
        {
            string result;
            try
            {
                var historialEmpleado = new HistorialEmpleado()
                {
                    EmpleadoId = empleadoId,
                    NuevoCambio = NuevoCambio,//Constantes.HISTORIALEMPLEADO_NUEVOCAMBIO_CREAR,
                    DatoAnterior = anterior, //"---",
                    Identificador = identificador, // Constantes.HISTORIALEMPLEADO_IDENTIFICADOR_CREAR
                };
                _historialEmpleadoServicio.Crear(historialEmpleado, _contextEmpleadoId);

                result = Constantes.RESPUESTA_OK;
            }
            catch (Exception e)
            {
                result = e.ToString();
            }

            return result;

        }


        #endregion
    }
}



