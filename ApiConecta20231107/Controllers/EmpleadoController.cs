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
using ApiBase.DTOs;

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

                var emplado = _empleadosServicio.ObtenerConsulta().FirstOrDefault(z => z.Id == Id);

                EmpleadoDTO result = new EmpleadoDTO();

                if (emplado == null)
                {
                    result.Nombre = "Invitado";
                    result.ApellidoPaterno = "";
                }
                else
                {
                    result.Nombre = emplado.Nombre;
                    result.ApellidoPaterno = emplado.ApellidoPaterno;
                }

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




        //Empleados Externos


        [HttpGet("ObetenerComboEmpleados")]
        public List<ComboDTO> Combos(string filtro)
        {

            var query = from t in ObtenerEmpleadosBrokers()
                        where (filtro =="0"|| t.BrokerId.ToString() == filtro)
                        select new ComboDTO()
                        {
                            id = t.Id.ToString(),
                            Nombre = t.NombreCompeto.ToString()
                        };
            return query.ToList();
        }


        public List<EmpleadoBrokerDTO> ObtenerEmpleadosBrokers()
        {
            List<EmpleadoBrokerDTO> listaEmpleados = new List<EmpleadoBrokerDTO>();
            Random random = new Random();

            Guid[] brokerIds = new Guid[]
            {
            Guid.Parse("3E6F7E7B-FF3F-4A8A-A9D2-153B792FCD13"),
            Guid.Parse("52DCC986-E111-44EE-B8CC-848E7CCF2033"),
            Guid.Parse("AA7D095F-81BF-461D-9E73-0BC991ECB695"),
            Guid.Parse("B448B4E2-F495-49DE-8118-67121AA39349"),
            Guid.Parse("26E1CFB9-AB69-4B4C-B3AE-AAFEB9FDB13B"),
            Guid.Parse("2CDD3B0B-887D-475A-854B-CF240323C79F"),
            Guid.Parse("94D253A9-51F8-4C98-95CB-8B88A4649CEB"),
            Guid.Parse("29443E71-5E00-48E7-9AD6-06B3AB45EB62"),
            Guid.Parse("498E1804-0F4B-45BE-AE32-EC2A21E507F2"),
            Guid.Parse("4B483132-368F-45FF-B067-17A1C1AC37CF"),
            Guid.Parse("D7D8CAF2-909B-488B-8B84-0CA69E260C74")
            };

            for (int i = 0; i < 40; i++)
            {
                EmpleadoBrokerDTO empleado = new EmpleadoBrokerDTO
                {
                    Id = Guid.NewGuid(),
                    BrokerId = brokerIds[random.Next(brokerIds.Length)],
                    NombreCompeto = $"Empleado {i + 1}"
                };
                listaEmpleados.Add(empleado);
            }


            return listaEmpleados;
        }



    }
}



