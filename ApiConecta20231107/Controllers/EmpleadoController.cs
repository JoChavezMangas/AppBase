using API.Auxiliares;
using API.DTOs;
using API.DTOs.OtrosSistemas;
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
using ApiConecta20231107.DTOs.OtrosSistemas;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ApiConecta20231107.Auxiliares;

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
        /// Metodo para crear empleados, tambien crea rol y añade claims
        /// </summary>
        /// <param name="empleadoDTO"></param>
        /// <returns></returns>
        [HttpPost("Crear")]
        //[Authorize(Roles = "RHEmpresa, RecursosHumanos, Usuario")]
        public async Task<JsonResult> CrearEmpelado([FromForm] EmpleadoDTO empleadoDTO)
        {
            string result = string.Empty;
            int param = 0;
            try
            {
                //Validar si el empleado ya ha sido creado

                var inBase = _empleadosServicio.ObtenerConsulta().Where(z => z.CURP == empleadoDTO.CURP || z.RFC == empleadoDTO.RFC||z.CorreoEmpresarial== empleadoDTO.CorreoEmpresarial);

                if (inBase.Any())
                {
                    if(inBase.Where(z => z.CURP == empleadoDTO.CURP).Any())
                        result= Constantes.RESPUESTA_CURP_DUPLICADO;
                    if (inBase.Where(z => z.RFC == empleadoDTO.RFC).Any())
                        result = Constantes.RESPUESTA_RFC_DUPLICADO;
                    if (inBase.Where(z => z.CorreoEmpresarial == empleadoDTO.CorreoEmpresarial).Any())
                        result = Constantes.RESPUESTA_CORREO_DUPLICADO;

                    return Json(new { status = result, param = 0 });
                }

                // Crea empleado en Multi login
                result = Constantes.RESPUESTA_ERROR_EMPLEADO_MULTILOGIN;
                empleadoDTO.idUser = Guid.NewGuid();
                var modeloEnviar = new MultiLoginDTO(empleadoDTO);   //MapModeloOtroSistema(Constantes.OTRO_SISTEMA_MULTILOGIN, empleadoDTO);
                var respuesta= _metodosAUX.PostWebAPiUsersMultiLogin(modeloEnviar, configuration["API_MULTILOGIN_signupallsystem"]);
                empleadoDTO.userName =  respuesta.username;

                // Crear Empleado
                result = Constantes.RESPUESTA_ERROR_EMPLEADO_DATOS;
                var empleado = mapper.Map<Empleado>(empleadoDTO);
                empleado.IdExterno = empleadoDTO.idUser;
                _empleadosServicio.Crear(empleado, empleadoDTO._contextEmpleadoId);

                //Crea registro que ya se registro en multi login
                if (!string.IsNullOrEmpty(respuesta.username))
                {
                    var modeloActivar = new API.DTOs.OtrosSistemas.User(empleadoDTO); //MapModeloOtroSistema(Constantes.MODELO_OTRO_SISTEMA_MULTILOGIN_ACTIVAR, empleadoDTO);
                    var respuesta2 = _metodosAUX.PostWebAPiUsersMultiLogin(modeloActivar, configuration["API_MULTILOGIN_changestatus"]);
                    CrearHistorialEmpleado(empleado.Id, Constantes.OTRO_SISTEMA_MULTILOGIN, string.Empty, "Sistemas", empleadoDTO._contextEmpleadoId);
                }

                param = empleado.Id;
                empleadoDTO.Id=param;

                // Crear Usuario con Rol y Claims
                CrearUsuarioRol(empleado, empleadoDTO.RolId, empleadoDTO.userName);

                // Registra historial de creacion
                CrearHistorialEmpleado(empleado.Id, Constantes.HISTORIALEMPLEADO_NUEVOCAMBIO_CREAR, "---", Constantes.HISTORIALEMPLEADO_IDENTIFICADOR_CREAR, empleadoDTO._contextEmpleadoId);

                //_metodosAUX.MandarCorreoNuevoUsuario("Josue Chavez", empleadoDTO.userName, empleadoDTO.userName, "SISEC");

                result = Constantes.RESPUESTA_OK;
                return Json(new { status = result, param = param, userName = empleadoDTO.userName });
            }
            catch (Exception ex)
            {
                return Json(new { status = result, param= param, userName= empleadoDTO.userName });
            }
            

        }



        /// <summary>
        /// Metodo para la bandeja de empleados, 
        /// </summary>
        /// <param name="filterValue0"></param>
        /// <param name="operatorValue0"></param>
        /// <param name="columnField0"></param>
        /// <returns>Lista filtrada de empleados</returns>
        [HttpGet("Listar")]
        //[Authorize(Roles = "RHEmpresa, RecursosHumanos")]
        public async Task<ActionResult<List<EmpleadoDTO>>> ObtenerEmpleado(string? filterValue0, string? operatorValue0, string? columnField0)
        {

            var result = ListaEmpleados().Result;

            // Valida que se haya mandado un parametro para comparar
            if (!string.IsNullOrEmpty(filterValue0))
            {
                if (columnField0 == "nombreCompleto")
                    result = result.Where(z => z.NombreCompleto.ToLower().Contains(filterValue0.ToLower()));
                if (columnField0 == "nombreEmpresa")
                    result = result.Where(z => z.NombreEmpresa.ToLower().Contains(filterValue0.ToLower()));
                if (columnField0 == "areaDepartamento")
                    result = result.Where(z => z.NombreArea.ToLower().Contains(filterValue0.ToLower()));
                if (columnField0 == "puesto")
                    result = result.Where(z => z.NombrePuesto.ToLower().Contains(filterValue0.ToLower()));
                if (columnField0 == "correoEmpresarial")
                    result = result.Where(z => z.CorreoEmpresarial.ToLower().Contains(filterValue0.ToLower()));
            }
            return result.ToList();
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

                EmpleadoDTO result = await ListaEmpleadosCompleto(Id);

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



        /// <summary>
        /// Metodo para la edicion de empleado, 
        /// registra en historial los cambios de Empresa, Area, Puesto, Rol o salario
        /// </summary>
        /// <param name="empleadoDTO"></param>
        /// <returns>Regresa texto de OK o motivo de por que no se actualizó</returns>
        [HttpPost("EditarEmpelado")]
        //[Authorize(Roles = "RHEmpresa, RecursosHumanos, Usuario")]
        public async Task<JsonResult> EditarEmpelado([FromForm] EmpleadoDTO empleadoDTO)
        {
            string result = string.Empty;
            try
            {

                var allInBase = _empleadosServicio.ObtenerConsulta().Where(z => z.CURP == empleadoDTO.CURP ||
                                                                            z.RFC == empleadoDTO.RFC ||
                                                                            z.CorreoEmpresarial == empleadoDTO.CorreoEmpresarial ||
                                                                            z.Id == empleadoDTO.Id);



                var inBase = allInBase.Where(z => (z.CURP == empleadoDTO.CURP ||
                                                                            z.RFC == empleadoDTO.RFC ||
                                                                            z.CorreoEmpresarial == empleadoDTO.CorreoEmpresarial)
                                                                            && z.Id != empleadoDTO.Id);

                var empleadoEnBase0 = allInBase.First(z => z.Id == empleadoDTO.Id);


                if (inBase.Any())
                {
                    if (inBase.Where(z => z.CURP == empleadoDTO.CURP).Any())
                        return Json(new { status = Constantes.RESPUESTA_CURP_DUPLICADO, param = empleadoDTO.Id, userName = empleadoDTO.userName });
                    if (inBase.Where(z => z.RFC == empleadoDTO.RFC).Any())
                        return Json(new { status = Constantes.RESPUESTA_RFC_DUPLICADO, param = empleadoDTO.Id, userName = empleadoDTO.userName });
                    if (inBase.Where(z => z.CorreoEmpresarial == empleadoDTO.CorreoEmpresarial).Any())
                        return Json(new { status = Constantes.RESPUESTA_CORREO_DUPLICADO, param = empleadoDTO.Id, userName = empleadoDTO.userName });
                }


                var empleado = mapper.Map<Empleado>(empleadoDTO);


                var empleadoEnBase = _empleadosServicio.ObtenerConsulta().FirstOrDefault(z => z.Id == empleadoDTO.Id);

                #region Actualiza al empleado
                empleado.FechaCreacion = empleadoEnBase.FechaCreacion;
                empleado.CreadoPor = empleadoEnBase.CreadoPor;
                empleado.IdExterno = empleadoEnBase.IdExterno;
                empleado.Sistemas = empleadoEnBase.Sistemas;
                _empleadosServicio.Actualizar(empleado, empleadoDTO._contextEmpleadoId);
                #endregion

                #region Actualiza empleados en otros sistemas
                EmpleadoSistemasDTO empleadoSistemasDTO = new EmpleadoSistemasDTO();
                empleadoSistemasDTO.id = empleadoDTO.Id;

                //Vailda si se ha cambiado el nombre, el curp o el RFC
                if (empleadoEnBase0.Nombre != empleadoDTO.Nombre
                   || empleadoEnBase0.SegundoNombre != empleadoDTO.SegundoNombre
                   || empleadoEnBase0.ApellidoPaterno != empleadoDTO.ApellidoPaterno
                   || empleadoEnBase0.ApellidoMaterno != empleadoDTO.ApellidoMaterno
                   || empleadoEnBase0.CURP != empleadoDTO.CURP
                   || empleadoEnBase0.RFC != empleadoDTO.RFC)
                {

                    var agenteDTO = empleadoEnBase0.Sistemas.Contains(Constantes.OTRO_SISTEMA_AGENTESOC) ?
                                    _metodosAUX.ObtenerOtroSistema(empleadoEnBase0.IdExterno.ToString(), configuration["API_EXTERNO_url"] + configuration["GETUSER_Agente"], Constantes.OTRO_SISTEMA_AGENTESOC) : DumyTask();
                    var brokerM = empleadoEnBase0.Sistemas.Contains(Constantes.OTRO_SISTEMA_BROKERMASTER) ?
                                    _metodosAUX.ObtenerOtroSistema(empleadoEnBase0.IdExterno.ToString(), configuration["API_EXTERNO_url"] + configuration["GETUSER_BrokerMaster"], Constantes.OTRO_SISTEMA_BROKERMASTER) : DumyTask();
                    var captura = empleadoEnBase0.Sistemas.Contains(Constantes.OTRO_SISTEMA_CAPTURA) ?
                                    _metodosAUX.ObtenerOtroSistema(empleadoEnBase0.IdExterno.ToString(), configuration["API_EXTERNO_url"] + configuration["GETUSER_CapturaVeloz"], Constantes.OTRO_SISTEMA_CAPTURA) : DumyTask();
                    var crediHip = empleadoEnBase0.Sistemas.Contains(Constantes.OTRO_SISTEMA_CREDIHIPO) ?
                                    _metodosAUX.ObtenerOtroSistema(empleadoEnBase0.IdExterno.ToString(), configuration["API_EXTERNO_url"] + configuration["GETUSER_CREDIHIPOTECARIO"], Constantes.OTRO_SISTEMA_CREDIHIPO) : DumyTask();
                    var resuelveDTO = empleadoEnBase0.Sistemas.Contains(Constantes.OTRO_SISTEMA_RESUELVEME) ?
                                    _metodosAUX.ObtenerOtroSistema(empleadoEnBase0.IdExterno.ToString(), configuration["API_EXTERNO_url"] + configuration["GETUSER_Resuelveme"], Constantes.OTRO_SISTEMA_RESUELVEME) : DumyTask();
                    var sicafiDTO = empleadoEnBase0.Sistemas.Contains(Constantes.OTRO_SISTEMA_SICAFI) ?
                                    _metodosAUX.ObtenerOtroSistema(empleadoEnBase0.IdExterno.ToString(), configuration["API_EXTERNO_url"] + configuration["GETUSER_SICAFI"], Constantes.OTRO_SISTEMA_SICAFI) : DumyTask();
                    var sisecDTO = empleadoEnBase0.Sistemas.Contains(Constantes.OTRO_SISTEMA_SISEC) ?
                                    _metodosAUX.ObtenerOtroSistema(empleadoEnBase0.IdExterno.ToString(), configuration["API_EXTERNO_url"] + configuration["GETUSER_Sisec"], Constantes.OTRO_SISTEMA_SISEC) : DumyTask();
                    var validocDTO = empleadoEnBase0.Sistemas.Contains(Constantes.OTRO_SISTEMA_VALIDOC) ?
                                    _metodosAUX.ObtenerOtroSistema(empleadoEnBase0.IdExterno.ToString(), configuration["API_EXTERNO_url"] + configuration["GETUSER_Validoc"], Constantes.OTRO_SISTEMA_VALIDOC) : DumyTask();


                    if (agenteDTO.Result is AgenteSOCDTO agente)
                        empleadoSistemasDTO.agenteRoles = agente.roleID;

                    if (brokerM.Result is BrokersMasterDTO brokerMaster)
                        empleadoSistemasDTO.brokerRoles = brokerMaster.roleID;

                    if (captura.Result is CapturaDTO cveloz)
                    {
                        empleadoSistemasDTO.cvelozAnios = string.Join(",", cveloz.years);
                        empleadoSistemasDTO.cvelozBancos = string.Join(",", cveloz.banks);
                        empleadoSistemasDTO.cvelozRoles = cveloz.roleID;
                    }

                    if (crediHip.Result is CrediHipotecarioDTO crhip)
                    {
                        empleadoSistemasDTO.crediRoles = crhip.roleID;
                        empleadoSistemasDTO.crediBancos = crhip.bankID;
                    }

                    if (resuelveDTO.Result is ResuelvemeDTO resuelveme)
                    {
                        empleadoSistemasDTO.resuleveDepartameto = resuelveme.departamentId;
                        empleadoSistemasDTO.resuelveEmpresa = resuelveme.companyId;
                        empleadoSistemasDTO.resuelveRoles = resuelveme.roleId;
                    }

                    if (sicafiDTO.Result is SicafiDTO scafi)
                    {
                        empleadoSistemasDTO.sicafiFranquicia = string.Join(",", scafi.franchise);
                        empleadoSistemasDTO.sicafiRoles = scafi.roleID;
                    }

                    if (sisecDTO.Result is SisecDTO sisc)
                    {
                        empleadoSistemasDTO.sisecFranquicia = string.Join(",", sisc.franchise);
                        empleadoSistemasDTO.sisecRoles = sisc.roleID;
                    }
                    if (validocDTO.Result is ValidocDTO validc)
                    {
                        empleadoSistemasDTO.validocOperacion = string.Join(",", validc.typeOperation);
                        empleadoSistemasDTO.validocRoles = validc.roleID;
                    }


                    empleadoSistemasDTO.sistemas = new string[] { empleadoEnBase0.Sistemas };

                    if (!string.IsNullOrEmpty(empleadoEnBase0.Sistemas))
                        MandarEmpeladoSistemas(empleadoSistemasDTO);

                }



                #endregion

                #region  Actualiza el rol y el usuario (en caso de no tenerlo, lo genera)
                var user = _userManager.Users.Where(z => z.Id == empleado.Id.ToString());
                if (empleadoDTO.RolId != null)
                {
                    if (user.Any())
                    {
                        var roles = await _userManager.GetRolesAsync(user.FirstOrDefault());
                        if (roles[0] != empleadoDTO.RolId)
                        {
                            _userManager.RemoveFromRolesAsync(user.FirstOrDefault(), roles);
                            //_userManager.RemoveClaimAsync (user.FirstOrDefault(), empleadoDTO.RolId);
                            string rolClaim = string.Empty;
                            switch (empleadoDTO.RolId)
                            {
                                case Constantes.ROLES_ADMIN:
                                    rolClaim = Constantes.ROLES_CLAIMS_ADMIN;
                                    break;
                                case Constantes.ROLES_OPERADOR:
                                    rolClaim = Constantes.ROLES_CLAIMS_OPERADOR; ;
                                    break;
                                case Constantes.ROLES_USUARIO:
                                    rolClaim = Constantes.ROLES_CLAIMS_USUARIO; ;
                                    break;
                            }
                            await _userManager.AddClaimAsync(user.FirstOrDefault(), new Claim(rolClaim, "1"));
                            await _userManager.AddToRoleAsync(user.FirstOrDefault(), empleadoDTO.RolId);

                            CrearHistorialEmpleado(empleado.Id, empleadoDTO.RolId, roles[0],
                                                    Constantes.HISTORIALEMPLEADO_IDENTIFICADOR_ROL, empleadoDTO._contextEmpleadoId);


                        }
                    }
                    else
                    {
                        //await CrearUsuarioRol(empleado, empleadoDTO.RolId);
                    }
                }

                #endregion



                result = Constantes.RESPUESTA_OK;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Json(new { status = result, param = empleadoDTO.Id, userName = empleadoDTO.userName });
        }





        /// <summary>
        /// Endpoint para mandar empleado a apiexternos.sisec
        /// </summary>
        /// <param name="modelo"></param>
        /// <returns></returns>
        [HttpPost("MandarEmpeladoSistemas")]
        //[Authorize(Roles = "RHEmpresa, RecursosHumanos, Usuario")]
        public async Task<JsonResult> MandarEmpeladoSistemas([FromForm] EmpleadoSistemasDTO modelo)
        {
            
            string result = string.Empty;
            
            try
            {

                //Tomamos la busqueda con un empelado, mas adelante la usaremos. 
                var query = _empleadosServicio.ObtenerConsulta().Where(t => t.Id == modelo.id).ToList();
                var empleado = query.First();
                var dtoList = from t in query
                              //join t2 in _tipoContratacionServicio.ObtenerLista()
                              //on t.Id equals t2.EmpleadoId
                              select new EmpleadoDTO
                              {
                                  Nombre=t.Nombre,
                                  SegundoNombre = t.SegundoNombre,
                                  ApellidoPaterno = t.ApellidoPaterno,
                                  ApellidoMaterno = t.ApellidoMaterno,
                                  RFC = t.RFC,
                                  CURP = t.CURP,
                                  CorreoEmpresarial = t.CorreoEmpresarial,
                                  telefono = "5599887744",//t2.telefono,
                                  idUser = t.IdExterno,
                                  empleadoSistemasDTO= modelo
                              };

                var dto = dtoList.FirstOrDefault();

                var usuarioMail = _userManager.FindByIdAsync(modelo.id.ToString()).Result;
                var usuarioUser = _userManager.FindByNameAsync(dto.CorreoEmpresarial).Result;
                var user = usuarioMail ?? usuarioUser;
                dto.userName = user.UserName;

                #region Creacion de usuario en otros sistemas

                var historial = _historialEmpleadoServicio.ObtenerLista().Where(z => z.EmpleadoId == modelo.id && z.Identificador== "Sistemas");

                string sistemas = modelo.sistemas[0] + empleado.Sistemas;


                string arraySistemas = string.Empty;
                string arrayCrecionUSuario = string.Empty;
                if (sistemas.Contains(Constantes.OTRO_SISTEMA_SISEC))
                {
                    bool algunavesSisec = historial.Where(z => z.NuevoCambio.Contains(Constantes.OTRO_SISTEMA_SISEC)).Any();
                    string endpointSISEC = algunavesSisec? configuration["API_EXTERNO_Sisec_UpdateUser"] : configuration["API_EXTERNO_Sisec_CreateUser"];
                    string metodo = algunavesSisec ? "put" : "post";
                    arrayCrecionUSuario = algunavesSisec ? arrayCrecionUSuario : arrayCrecionUSuario + Constantes.OTRO_SISTEMA_SISEC;
                    GeneralDTO modeloSISEC = new SisecDTO(dto);
                    string respuestaSISEC = _metodosAUX.MandarOtroSistema(modeloSISEC, configuration["API_EXTERNO_url"] + endpointSISEC, metodo);
                    arraySistemas = respuestaSISEC == Constantes.RESPUESTA_OK && modelo.sistemas[0].Contains(Constantes.OTRO_SISTEMA_SISEC) ? arraySistemas + Constantes.OTRO_SISTEMA_SISEC : arraySistemas;
                }
                if (sistemas.Contains(Constantes.OTRO_SISTEMA_AGENTESOC))
                {
                    bool algunavesAgente = historial.Where(z => z.NuevoCambio.Contains(Constantes.OTRO_SISTEMA_AGENTESOC)).Any();
                    string endpointAgente = algunavesAgente ? configuration["API_EXTERNO_AgenteSOC_UpdateUser"] : configuration["API_EXTERNO_AgenteSOC_CreateUser"];
                    string metodo = algunavesAgente ? "put" : "post";
                    arrayCrecionUSuario = algunavesAgente ? arrayCrecionUSuario : arrayCrecionUSuario + Constantes.OTRO_SISTEMA_AGENTESOC;
                    GeneralDTO modeloAgenteSOC = new AgenteSOCDTO(dto); //MapModeloOtroSistema(Constantes.OTRO_SISTEMA_AGENTESOC, dto);
                    string respuestaAGENTESOC = _metodosAUX.MandarOtroSistema(modeloAgenteSOC, configuration["API_EXTERNO_url"] + endpointAgente, metodo);
                    arraySistemas = respuestaAGENTESOC == Constantes.RESPUESTA_OK && modelo.sistemas[0].Contains(Constantes.OTRO_SISTEMA_AGENTESOC) ? arraySistemas + Constantes.OTRO_SISTEMA_AGENTESOC : arraySistemas;
                }
                if (sistemas.Contains(Constantes.OTRO_SISTEMA_BROKERMASTER))
                {
                    bool algunavesBrokerM = historial.Where(z => z.NuevoCambio.Contains(Constantes.OTRO_SISTEMA_BROKERMASTER)).Any();
                    string endpointBrokerM = algunavesBrokerM ? configuration["API_EXTERNO_BrokerMaster_UpdateUser"] : configuration["API_EXTERNO_BrokerMaster_CreateUser"];
                    string metodo = algunavesBrokerM ? "put" : "post";
                    arrayCrecionUSuario = algunavesBrokerM ? arrayCrecionUSuario : arrayCrecionUSuario + Constantes.OTRO_SISTEMA_BROKERMASTER;
                    GeneralDTO modeloAgenteBrokerMaster = new BrokersMasterDTO(dto); //MapModeloOtroSistema(Constantes.OTRO_SISTEMA_BROKERMASTER, dto);
                    string respuestaBROKERMASTER = _metodosAUX.MandarOtroSistema(modeloAgenteBrokerMaster, configuration["API_EXTERNO_url"] + endpointBrokerM,metodo);
                    arraySistemas = respuestaBROKERMASTER == Constantes.RESPUESTA_OK && modelo.sistemas[0].Contains(Constantes.OTRO_SISTEMA_BROKERMASTER) ? arraySistemas + Constantes.OTRO_SISTEMA_BROKERMASTER : arraySistemas;
                }
                if (sistemas.Contains(Constantes.OTRO_SISTEMA_CAPTURA))
                {
                    bool algunavesCaptura = historial.Where(z => z.NuevoCambio.Contains(Constantes.OTRO_SISTEMA_CAPTURA)).Any();
                    string endpointCaptura = algunavesCaptura ? configuration["API_EXTERNO_CapturaVeloz_UpdateUser"] : configuration["API_EXTERNO_CapturaVeloz_CreateUser"];
                    string metodo = algunavesCaptura ? "put" : "post";
                    arrayCrecionUSuario = algunavesCaptura ? arrayCrecionUSuario : arrayCrecionUSuario + Constantes.OTRO_SISTEMA_CAPTURA;
                    GeneralDTO modeloAgenteCaptura = new CapturaDTO(dto); //MapModeloOtroSistema(Constantes.OTRO_SISTEMA_CAPTURA, dto);
                    string respuestaCAPTURA = _metodosAUX.MandarOtroSistema(modeloAgenteCaptura, configuration["API_EXTERNO_url"] + endpointCaptura,metodo);
                    arraySistemas = respuestaCAPTURA == Constantes.RESPUESTA_OK && modelo.sistemas[0].Contains(Constantes.OTRO_SISTEMA_CAPTURA) ? arraySistemas + Constantes.OTRO_SISTEMA_CAPTURA : arraySistemas;
                }
                if (sistemas.Contains(Constantes.OTRO_SISTEMA_CREDIHIPO))
                {
                    bool algunavesCredihipo = historial.Where(z => z.NuevoCambio.Contains(Constantes.OTRO_SISTEMA_CREDIHIPO)).Any();
                    string endpointCredihipo = algunavesCredihipo ? configuration["API_EXTERNO_CREDIHIPOTECARIO_UpdateUser"] : configuration["API_EXTERNO_CREDIHIPOTECARIO_CreateUser"];
                    string metodo = algunavesCredihipo ? "put" : "post";
                    arrayCrecionUSuario = algunavesCredihipo ? arrayCrecionUSuario : arrayCrecionUSuario + Constantes.OTRO_SISTEMA_CREDIHIPO;
                    GeneralDTO modeloAgenteCrediHipo = new CrediHipotecarioDTO(dto); //MapModeloOtroSistema(Constantes.OTRO_SISTEMA_CREDIHIPO, dto);
                    string respuestaCREDIHIPO = _metodosAUX.MandarOtroSistema(modeloAgenteCrediHipo, configuration["API_EXTERNO_url"] + endpointCredihipo, metodo);
                    arraySistemas = respuestaCREDIHIPO == Constantes.RESPUESTA_OK && modelo.sistemas[0].Contains(Constantes.OTRO_SISTEMA_CREDIHIPO) ? arraySistemas + Constantes.OTRO_SISTEMA_CREDIHIPO : arraySistemas;
                }
                if (sistemas.Contains(Constantes.OTRO_SISTEMA_RESUELVEME))
                {
                    bool algunavesResuelveme = historial.Where(z => z.NuevoCambio.Contains(Constantes.OTRO_SISTEMA_RESUELVEME)).Any();
                    string endpointResuelveme = algunavesResuelveme ? configuration["API_EXTERNO_Resuelveme_UpdateUser"] : configuration["API_EXTERNO_Resuelveme_CreateUser"];
                    string metodo = algunavesResuelveme ? "put" : "post";
                    arrayCrecionUSuario = algunavesResuelveme ? arrayCrecionUSuario : arrayCrecionUSuario + Constantes.OTRO_SISTEMA_RESUELVEME;
                    GeneralDTO modeloAgenteResuelveme = new ResuelvemeDTO(dto); //MapModeloOtroSistema(Constantes.OTRO_SISTEMA_RESUELVEME, dto);
                    string respuestaRESUELVEME = _metodosAUX.MandarOtroSistema(modeloAgenteResuelveme, configuration["API_EXTERNO_url"] + endpointResuelveme,metodo);
                    arraySistemas = respuestaRESUELVEME == Constantes.RESPUESTA_OK&& modelo.sistemas[0].Contains(Constantes.OTRO_SISTEMA_RESUELVEME) ? arraySistemas + Constantes.OTRO_SISTEMA_RESUELVEME : arraySistemas;
                }
                if (sistemas.Contains(Constantes.OTRO_SISTEMA_SICAFI))
                {
                    bool algunavesSicafi = historial.Where(z => z.NuevoCambio.Contains(Constantes.OTRO_SISTEMA_SICAFI)).Any();
                    string endpointSicafi = algunavesSicafi ? configuration["API_EXTERNO_SICAFI_UpdateUser"] : configuration["API_EXTERNO_SICAFI_CreateUser"];
                    string metodo = algunavesSicafi ? "put" : "post";
                    arrayCrecionUSuario = algunavesSicafi ? arrayCrecionUSuario : arrayCrecionUSuario + Constantes.OTRO_SISTEMA_SICAFI;
                    GeneralDTO modeloAgenteSICAFI = new SicafiDTO(dto); //MapModeloOtroSistema(Constantes.OTRO_SISTEMA_SICAFI, dto);
                    string respuestaSICAFI = _metodosAUX.MandarOtroSistema(modeloAgenteSICAFI, configuration["API_EXTERNO_url"] + endpointSicafi,metodo);
                    arraySistemas = respuestaSICAFI == Constantes.RESPUESTA_OK&& modelo.sistemas[0].Contains(Constantes.OTRO_SISTEMA_SICAFI) ? arraySistemas + Constantes.OTRO_SISTEMA_SICAFI : arraySistemas;
                }
                if (sistemas.Contains(Constantes.OTRO_SISTEMA_VALIDOC))
                {
                    bool algunavesSisec = historial.Where(z => z.NuevoCambio.Contains(Constantes.OTRO_SISTEMA_SISEC)).Any();
                    string endpointSISEC = algunavesSisec ? configuration["API_EXTERNO_Sisec_UpdateUser"] : configuration["API_EXTERNO_Sisec_CreateUser"];
                    string metodo = algunavesSisec ? "put" : "post";
                    arrayCrecionUSuario = algunavesSisec ? arrayCrecionUSuario : arrayCrecionUSuario + Constantes.OTRO_SISTEMA_VALIDOC;

                    GeneralDTO modeloAgenteValidoc = new ValidocDTO(dto); //MapModeloOtroSistema(Constantes.OTRO_SISTEMA_VALIDOC, dto);
                    string respuestaVALIDOC = _metodosAUX.MandarOtroSistema(modeloAgenteValidoc, "https://c37c-187-189-91-228.ngrok-free.app");
                    arraySistemas = respuestaVALIDOC == Constantes.RESPUESTA_OK && modelo.sistemas[0].Contains(Constantes.OTRO_SISTEMA_VALIDOC) ? arraySistemas + Constantes.OTRO_SISTEMA_VALIDOC : arraySistemas;
                }
                #endregion

                
                empleado.Sistemas = arraySistemas;
                _empleadosServicio.Actualizar(empleado, modelo._contextEmpleadoId);

                CrearHistorialEmpleado(empleado.Id, arraySistemas,string.Empty ,"Sistemas", modelo._contextEmpleadoId);

                if (!string.IsNullOrEmpty(arrayCrecionUSuario))
                {
                    string nombreContacto = dto.Nombre + " " + dto.ApellidoPaterno + " " + dto.ApellidoMaterno;
                    _metodosAUX.MandarCorreoNuevoUsuario(nombreContacto, dto.userName, dto.CorreoEmpresarial, arraySistemas);
                }

                result = Constantes.RESPUESTA_OK;

            }
            catch (Exception ex) 
            {
                result= ex.Message;
            }

            return Json(new { status = result });
        }

        
        public async Task<GeneralDTO> DumyTask()
        {
            GeneralDTO result = new GeneralDTO();
            return result;
        }
        
        






        #region Metodos auxiliares para controlador




        /// <summary>
        /// Busqueda general de empleados, obtiene Empleados con puestos areas y empresas
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<EmpleadoDTO>> ListaEmpleados()
        {
            var consulta = from empleados in _empleadosServicio.ObtenerConsulta()
                           //join puesto in _context.Puestos.AsQueryable()
                           //on empleados.PuestoId equals puesto.Id
                           //join areas in _context.Areas.AsQueryable()
                           //on puesto.AreaId equals areas.Id
                           //join empresa in _context.Empresas.AsQueryable()
                           //on areas.EmpresaId equals empresa.Id

                           select new EmpleadoDTO
                           {
                               Id = empleados.Id,
                               Nombre = empleados.Nombre,
                               SegundoNombre = empleados.SegundoNombre,
                               ApellidoPaterno = empleados.ApellidoPaterno,
                               ApellidoMaterno = empleados.ApellidoMaterno,
                               NombreCompleto = empleados.Nombre + " " +
                                                empleados.SegundoNombre + " " +
                                                empleados.ApellidoPaterno + " " +
                                                empleados.ApellidoMaterno,
                               NombreEmpresa = "Nombre de la empresa",
                               NombreArea = "Nombre del area",
                               NombrePuesto = "Nombre del puesto",
                               CorreoEmpresarial = empleados.CorreoEmpresarial,
                               CorreoPersonal = empleados.CorreoPersonal,
                               CURP = empleados.CURP,
                               RFC = empleados.RFC,
                               FechaNacimiento = empleados.FechaNacimiento,
                               Sexo = empleados.Sexo,
                               AreaId = 0,
                               EmpresaId = 0,
                               PuestoId = 0,
                               SueldoDiario = empleados.SueldoDiario,
                               SueldoDiarioIntegrado = empleados.SueldoDiarioIntegrado,
                           };


            return consulta;
        }


        /// <summary>
        /// Busqueda general de empleados con toda la informacion
        /// </summary>
        /// <returns></returns>
        public async Task<EmpleadoDTO> ListaEmpleadosCompleto(int empleadoId)
        {
            #region consulta anterior
            var consulta = from empleados in _empleadosServicio.ObtenerConsulta() //_context.Empleados.AsQueryable()
                          

                           where empleados.Id == empleadoId

                           select new EmpleadoDTO
                           {
                               Id = empleados.Id,
                               Nombre = empleados.Nombre,
                               SegundoNombre = empleados.SegundoNombre,
                               ApellidoPaterno = empleados.ApellidoPaterno,
                               ApellidoMaterno = empleados.ApellidoMaterno,
                               NombreCompleto = empleados.Nombre + " " +
                                                empleados.SegundoNombre + " " +
                                                empleados.ApellidoPaterno + " " +
                                                empleados.ApellidoMaterno,
                               NombreEmpresa = "",
                               NombreArea = "",
                               NombrePuesto = "",
                               CorreoEmpresarial = empleados.CorreoEmpresarial,
                               CorreoPersonal = empleados.CorreoPersonal,
                               CURP = empleados.CURP,
                               RFC = empleados.RFC,
                               FechaNacimiento = empleados.FechaNacimiento,
                               Sexo = empleados.Sexo,
                               AreaId = 0,
                               EmpresaId = 0,
                               PuestoId = 0,
                               SueldoDiario = empleados.SueldoDiario,
                               SueldoDiarioIntegrado = empleados.SueldoDiarioIntegrado,
                               idUser = empleados.IdExterno,


                               //Tipo Contratacion
                               nss = "",
                               telefono = "",
                               fechaIngreso = DateTime.Now,
                               fechaTerminacionPrueba = DateTime.Now,
                               modalidad = "",

                               //Direccion
                               pais = "",
                               estado = "",
                               municipio = "",
                               colonia = "",
                               calle = "",
                               codigoPostal = "",
                               telefonoFijo = "",


                               //Datos bancarios
                               NoCuenta = "",
                               clabe = "",
                               bancoId = 0,
                               bancoExterno = Guid.Empty,
                               numeroTarjeta = "",



                               //Nomina
                               //SueldoDiario = x3 == null ? 0 : x3.sueldoDiario,
                               //SueldoDiarioIntegrado = x3 == null ? 0 : x3.sueldoDiarioIntegrado,
                               BonoTrimestral = 0,
                               fechaSueldo = DateTime.Now,
                               tipoContrato = "",
                               duracionContrato = "",
                               finContrato = null,
                               tipoRegimen = string.Empty,
                               razonSocial = string.Empty,
                               esquemaOutsourcing = false,


                               //EmpleadoContactoEmergecia
                               nombreContacto = string.Empty,
                               parentesco = string.Empty,
                               celularContacto = string.Empty,
                               telefonoContacto = string.Empty,
                               correoContacto = string.Empty,

                               //Lider directo
                               liderDirecto = 0
                           };
            #endregion

            var obj = consulta.FirstOrDefault();

            return obj;
        }


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



