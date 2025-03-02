using API.DTOs;
using ApiBase.DTOs.ReportesDTO;
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
    public class ReportesController : Controller
    {

        private readonly IPedidosServicio _pedidosServicio;
        private readonly ICatalogosServicio _catalogosServicio;
        private readonly IBancoServicio _bancoServicio;
        private readonly IBrokersServicio _brokersServicio;


        public ReportesController(  IPedidosServicio pedidosServicio,
                                    ICatalogosServicio catalogosServicio,
                                    IBancoServicio bancoServicio,
                                    IBrokersServicio brokersServicio)
        {
            this._pedidosServicio = pedidosServicio;
            this._catalogosServicio = catalogosServicio;
            this._bancoServicio = bancoServicio;
            this._brokersServicio = brokersServicio;
        }


        [HttpGet("ColocacionMensual")]
        public async Task<ActionResult<List<ReporteColocacionMensulaDTO>>> ColocacionMensual(string? filterValue0, string? operatorValue0, string? columnField0,string genericParam)
        {
            var parametrosArray = genericParam.Split('#');

            var catalogos = _catalogosServicio.ObtenerConsulta().Where(z => z.Tipo == "Año").ToList();



            var anio = parametrosArray[1] == ""?"": catalogos.Where(z=>z.Id.ToString()== parametrosArray[1]).FirstOrDefault().Valor;
            var mes = parametrosArray[2];
            var broker = parametrosArray[3];
            var ejecutivo = parametrosArray[4];
            var estado = parametrosArray[5];

            var query = _pedidosServicio.ObtenerConsulta()
                 .Where(p => p.FechaCreacion.HasValue)
                 .Where(z=> (anio == ""|| z.FechaCreacion.Value.Year.ToString()==anio)&&
                            (mes == "" || z.FechaCreacion.Value.Month.ToString() == mes) &&
                            (broker == "" || z.Broker.ToString() == broker)&&
                            (estado == "" || z.Estado.ToString() == estado))
                 .GroupBy(p => new { p.FechaCreacion.Value.Year, p.FechaCreacion.Value.Month })
                 .Select(g => new ColocacionMensulaDTO
                 {
                     anio = g.Key.Year,
                     mes = g.Key.Month,
                     monto = (double)g.Sum(p => p.Monto)
                 })
                 .ToList();

            // Lista de meses en español
            var meses = new Dictionary<int, string>
            {
                { 1, "Enero" }, { 2, "Febrero" }, { 3, "Marzo" }, { 4, "Abril" },
                { 5, "Mayo" }, { 6, "Junio" }, { 7, "Julio" }, { 8, "Agosto" },
                { 9, "Septiembre" }, { 10, "Octubre" }, { 11, "Noviembre" }, { 12, "Diciembre" }
            };

            // Obtener lista de años desde los datos
            var anios = query.Select(q => q.anio).Distinct().OrderByDescending(a => a).ToList();

            var resultados = meses.Select(m => new ReporteColocacionMensulaDTO
            {
                id = Guid.NewGuid().ToString(),
                mes = m.Value,
                a2025 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2025)?.monto.ToString("F2") ?? "0.00",
                a2024 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2024)?.monto.ToString("F2") ?? "0.00",
                a2023 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2023)?.monto.ToString("F2") ?? "0.00",
                a2022 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2022)?.monto.ToString("F2") ?? "0.00",
                a2021 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2021)?.monto.ToString("F2") ?? "0.00",
                a2020 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2020)?.monto.ToString("F2") ?? "0.00",
                total = query.Where(q => q.mes == m.Key).Sum(q => q.monto).ToString("F2")
            }).ToList();

            return resultados;


        }


        [HttpGet("OperacionesMensual")]
        public async Task<ActionResult<List<ReporteColocacionMensulaDTO>>> OperacionesMensual(string? filterValue0, string? operatorValue0, string? columnField0, string genericParam)
        {
            var parametrosArray = genericParam.Split('#');

            var catalogos = _catalogosServicio.ObtenerConsulta().Where(z => z.Tipo == "Año").ToList();



            var anio = parametrosArray[1] == "" ? "" : catalogos.Where(z => z.Id.ToString() == parametrosArray[1]).FirstOrDefault().Valor;
            var mes = parametrosArray[2];
            var broker = parametrosArray[3];
            var ejecutivo = parametrosArray[4];
            var estado = parametrosArray[5];

            var query = _pedidosServicio.ObtenerConsulta()
                 .Where(p => p.FechaCreacion.HasValue)
                 .Where(z => (anio == "" || z.FechaCreacion.Value.Year.ToString() == anio) &&
                            (mes == "" || z.FechaCreacion.Value.Month.ToString() == mes) &&
                            (broker == "" || z.Broker.ToString() == broker) &&
                            (estado == "" || z.Estado.ToString() == estado))
                 .GroupBy(p => new { p.FechaCreacion.Value.Year, p.FechaCreacion.Value.Month })
                 .Select(g => new ColocacionMensulaDTO
                 {
                     anio = g.Key.Year,
                     mes = g.Key.Month,
                     operaciones = g.Count()
                 })
                 .ToList();

            // Lista de meses en español
            var meses = new Dictionary<int, string>
            {
                { 1, "Enero" }, { 2, "Febrero" }, { 3, "Marzo" }, { 4, "Abril" },
                { 5, "Mayo" }, { 6, "Junio" }, { 7, "Julio" }, { 8, "Agosto" },
                { 9, "Septiembre" }, { 10, "Octubre" }, { 11, "Noviembre" }, { 12, "Diciembre" }
            };

            // Obtener lista de años desde los datos
            var anios = query.Select(q => q.anio).Distinct().OrderByDescending(a => a).ToList();

            var resultados = meses.Select(m => new ReporteColocacionMensulaDTO
            {
                id = Guid.NewGuid().ToString(),
                mes = m.Value,
                a2025 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2025)?.operaciones.ToString() ?? "0",
                a2024 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2024)?.operaciones.ToString() ?? "0",
                a2023 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2023)?.operaciones.ToString() ?? "0",
                a2022 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2022)?.operaciones.ToString() ?? "0",
                a2021 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2021)?.operaciones.ToString() ?? "0",
                a2020 = query.FirstOrDefault(q => q.mes == m.Key && q.anio == 2020)?.operaciones.ToString() ?? "0",
                total = query.Where(q => q.mes == m.Key).Sum(q => q.operaciones).ToString()
            }).ToList();

            return resultados;


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
    }
}
