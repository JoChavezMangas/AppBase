using API.Auxiliares;
using API.DTOs;
using ApiBase.Auxiliares;
using ApiBase.DTOs;
using Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servicios;

namespace ApiBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PedidosController : Controller
    {
        private readonly IEmpleadosServicio _empleadosServicio;
        private readonly ICatalogosServicio _catalogosServicio;
        private readonly IBancoServicio _bancoServicio;
        private readonly IBrokersServicio _brokersServicio;
        private readonly ILogApiServicio _logApiServicio;
        private readonly IPedidosServicio _pedidosServicio;
        private readonly IContextoPeticion _contextoPeticion;

        public PedidosController(IEmpleadosServicio empleadosServicio,
                                 ICatalogosServicio catalogosServicio,
                                 IBancoServicio bancoServicio,
                                 IBrokersServicio brokersServicio,
                                 ILogApiServicio logApiServicio,
                                 IPedidosServicio pedidosServicio,
                                 IContextoPeticion contextoPeticion)
        {
            this._empleadosServicio = empleadosServicio;
            this._catalogosServicio = catalogosServicio;
            this._bancoServicio = bancoServicio;
            this._brokersServicio = brokersServicio;
            this._logApiServicio = logApiServicio;
            this._pedidosServicio = pedidosServicio;
            this._contextoPeticion = contextoPeticion;
        }


        [HttpPost("RegistrarPedidos")]
        public async Task<JsonResult> RegistrarPedidos([FromForm] PedidoDTO pedidoDTO)
        {
            string result = string.Empty;

            try
            {
                _pedidosServicio.Crear(pedidoDTO.PedidoParaBase(), _contextoPeticion.empleadoId);
                result = Constantes.RESPUESTA_OK;
            }
            catch (Exception ex)
            {
                LogApi log = new LogApi()
                {
                    status = ex.Source,
                    urlEnpoint = ex.StackTrace,
                    Message = ex.Message,
                    json = ex.InnerException.ToString(),
                };
                _logApiServicio.Crear(log,"");

                result = ex.Message;
                return Json(new { status = Constantes.RESPUESTA_ERROR, message = ex.Message });
            }

            return Json(new { status = result, message= Constantes.RESPUESTA_MESSAGE });

        }




        [HttpGet("ObetenerCombo")]
        public List<ComboDTO> Combos(string filtro)
        {

            var query = from t in _catalogosServicio.ObtenerConsulta()
                        where t.Tipo == filtro
                        select new ComboDTO()
                        {
                            id = t.Id.ToString(),
                            Nombre = t.Nombre.ToString()
                        };
            return query.ToList();
        }


        [HttpGet("ObtenerBancos")]
        public List<ComboDTO> ObtenerBancos(string filtro)
        {
            var query = from t in _bancoServicio.ObtenerConsulta()
                        select new ComboDTO()
                        {
                            id = t.Id.ToString(),
                            Nombre = t.Nombre.ToString()
                        };
            return query.ToList();
        }


        [HttpGet("ObtenerBrokers")]
        public List<ComboDTO> ObtenerBrokers(string filtro)
        {
            var query = from t in _brokersServicio.ObtenerConsulta()
                        select new ComboDTO()
                        {
                            id = t.Id.ToString(),
                            Nombre = t.NombreComercial.ToString()
                        };
            return query.ToList();
        }


    }
}
