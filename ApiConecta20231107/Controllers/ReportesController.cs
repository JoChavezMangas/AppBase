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

        [HttpGet("ColocacionBancos")]
        public async Task<ActionResult<List<ReporteColocacionBancoDTO>>> ColocacionBancos(string? filterValue0, string? operatorValue0, string? columnField0, string genericParam)
        {
            var parametrosArray = genericParam.Split('#');

            var catalogos = _catalogosServicio.ObtenerConsulta().Where(z => z.Tipo == "Año").ToList();



            var anio = parametrosArray[1] == "" ? "" : catalogos.Where(z => z.Id.ToString() == parametrosArray[1]).FirstOrDefault().Valor;
            var mes = parametrosArray[2];
            var broker = parametrosArray[3];
            var ejecutivo = parametrosArray[4];
            var estado = parametrosArray[5];




            var queryBase = _pedidosServicio.ObtenerConsulta()
                .Where(p => p.FechaCreacion.HasValue)
                .Where(z => (anio == "" || z.FechaCreacion.Value.Year.ToString() == anio) &&
                            (mes == "" || z.FechaCreacion.Value.Month.ToString() == mes) &&
                            (broker == "" || z.Broker.ToString() == broker) &&
                            (estado == "" || z.Estado.ToString() == estado))
                .ToList(); // Convertir a lista para evitar múltiples consultas

            // Obtener la suma total de montos por año
            var totalPorAnio = queryBase
                .GroupBy(p => p.FechaCreacion.Value.Year)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Monto));

            // Obtener la suma total de montos de todos los años
            var totalGlobal = totalPorAnio.Values.Sum();

            // Agrupar por año y banco, y calcular el porcentaje
            var datosAgrupados = queryBase
                .GroupBy(p => new { p.FechaCreacion.Value.Year, p.Banco })
                .Select(g => new
                {
                    anio = g.Key.Year,
                    banco = g.Key.Banco,
                    monto = g.Sum(z => z.Monto),
                    porcentaje = totalPorAnio.ContainsKey(g.Key.Year) && totalPorAnio[g.Key.Year] > 0
                        ? (double)(g.Sum(z => z.Monto) / totalPorAnio[g.Key.Year]) * 100
                        : 0.0
                })
                .ToList();


            var bancos = _bancoServicio.ObtenerConsulta().ToList();

            // Generar la lista final en el formato del DTO
            var resultados = datosAgrupados
                .GroupBy(d => d.banco)
                .Select(g => new ReporteColocacionBancoDTO
                {
                    id = Guid.NewGuid().ToString(),
                    banco = bancos.FirstOrDefault(z => z.Id == g.Key)?.Nombre ?? "Desconocido",
                    a2025 = g.FirstOrDefault(x => x.anio == 2025)?.porcentaje.ToString("F2") ?? "0.00",
                    a2024 = g.FirstOrDefault(x => x.anio == 2024)?.porcentaje.ToString("F2") ?? "0.00",
                    a2023 = g.FirstOrDefault(x => x.anio == 2023)?.porcentaje.ToString("F2") ?? "0.00",
                    a2022 = g.FirstOrDefault(x => x.anio == 2022)?.porcentaje.ToString("F2") ?? "0.00",
                    a2021 = g.FirstOrDefault(x => x.anio == 2021)?.porcentaje.ToString("F2") ?? "0.00",
                    a2020 = g.FirstOrDefault(x => x.anio == 2020)?.porcentaje.ToString("F2") ?? "0.00",
                    total = totalGlobal > 0
                        ? (g.Sum(x => x.monto) / totalGlobal * 100).ToString("F2") // Suma total del banco sobre la suma global
                        : "0.00"
                })
                .ToList();

            return resultados;



        }

        [HttpGet("ColocacionEstados")]
        public async Task<ActionResult<List<ReporteColocacionEstadoDTO>>> ColocacionEstados(string? filterValue0, string? operatorValue0, string? columnField0, string genericParam)
        {
            var parametrosArray = genericParam.Split('#');

            var catalogos = _catalogosServicio.ObtenerConsulta().Where(z => z.Tipo == "Año").ToList();



            var anio = parametrosArray[1] == "" ? "" : catalogos.Where(z => z.Id.ToString() == parametrosArray[1]).FirstOrDefault().Valor;
            var mes = parametrosArray[2];
            var broker = parametrosArray[3];
            var ejecutivo = parametrosArray[4];
            var estado = parametrosArray[5];




            var queryBase = _pedidosServicio.ObtenerConsulta()
                .Where(p => p.FechaCreacion.HasValue)
                .Where(z => (anio == "" || z.FechaCreacion.Value.Year.ToString() == anio) &&
                            (mes == "" || z.FechaCreacion.Value.Month.ToString() == mes) &&
                            (broker == "" || z.Broker.ToString() == broker) &&
                            (estado == "" || z.Estado.ToString() == estado))
                .ToList(); // Convertir a lista para evitar múltiples consultas

            // Obtener la suma total de montos por año
            var totalPorAnio = queryBase
                .GroupBy(p => p.FechaCreacion.Value.Year)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Monto));

            // Obtener la suma total de montos de todos los años
            var totalGlobal = totalPorAnio.Values.Sum();

            // Agrupar por año y banco, y calcular el porcentaje
            var datosAgrupados = queryBase
                .GroupBy(p => new { p.FechaCreacion.Value.Year, p.Estado })
                .Select(g => new
                {
                    anio = g.Key.Year,
                    Estado = g.Key.Estado,
                    monto = g.Sum(z => z.Monto),
                    porcentaje = totalPorAnio.ContainsKey(g.Key.Year) && totalPorAnio[g.Key.Year] > 0
                        ? (double)(g.Sum(z => z.Monto) / totalPorAnio[g.Key.Year]) * 100
                        : 0.0
                })
                .ToList();


            var estados = _catalogosServicio.ObtenerConsulta().Where(z => z.Tipo == "Estado");

            // Generar la lista final en el formato del DTO
            var resultados = datosAgrupados
                .GroupBy(d => d.Estado)
                .Select(g => new ReporteColocacionEstadoDTO
                {
                    id = Guid.NewGuid().ToString(),
                    estado = estados.FirstOrDefault(z => z.Id == g.Key)?.Nombre ?? "Desconocido",
                    a2025 = g.FirstOrDefault(x => x.anio == 2025)?.porcentaje.ToString("F2") ?? "0.00",
                    a2024 = g.FirstOrDefault(x => x.anio == 2024)?.porcentaje.ToString("F2") ?? "0.00",
                    a2023 = g.FirstOrDefault(x => x.anio == 2023)?.porcentaje.ToString("F2") ?? "0.00",
                    a2022 = g.FirstOrDefault(x => x.anio == 2022)?.porcentaje.ToString("F2") ?? "0.00",
                    a2021 = g.FirstOrDefault(x => x.anio == 2021)?.porcentaje.ToString("F2") ?? "0.00",
                    a2020 = g.FirstOrDefault(x => x.anio == 2020)?.porcentaje.ToString("F2") ?? "0.00",
                    total = totalGlobal > 0
                        ? (g.Sum(x => x.monto) / totalGlobal * 100).ToString("F2") // Suma total del banco sobre la suma global
                        : "0.00"
                })
                .ToList();

            return resultados;
        }












        [HttpGet("CrecimientoOperaciones")]
        public async Task<ActionResult<List<CrecimientoOperacionesDTO>>> CrecimientoOperaciones(string? filterValue0, string? operatorValue0, string? columnField0, string genericParam)
        {
            var parametrosArray = genericParam.Split('#');

            var catalogos = _catalogosServicio.ObtenerConsulta().Where(z => z.Tipo == "Año").ToList();



            var anio = parametrosArray[1] == "" ? "" : catalogos.Where(z => z.Id.ToString() == parametrosArray[1]).FirstOrDefault().Valor;
            var mes = parametrosArray[2];
            var broker = parametrosArray[3];
            var ejecutivo = parametrosArray[4];
            var estado = parametrosArray[5];




            var queryBase = _pedidosServicio.ObtenerConsulta()
                .Where(p => p.FechaCreacion.HasValue)
                .Where(z => (anio == "" || z.FechaCreacion.Value.Year.ToString() == anio) &&
                            (mes == "" || z.FechaCreacion.Value.Month.ToString() == mes) &&
                            (broker == "" || z.Broker.ToString() == broker) &&
                            (estado == "" || z.Estado.ToString() == estado))
                .ToList(); // Convertir a lista para evitar múltiples consultas






            // Definir las metas por año
            var metasPorAnio = new Dictionary<int, int>
                                {
                                    { 2020, 15 },
                                    { 2021, 17 },
                                    { 2022, 19 },
                                    { 2023, 21 },
                                    { 2024, 23 },
                                    { 2025, 25 }
                                };

            var operacionesPorAnio = queryBase
                .Where(p => p.FechaCreacion.HasValue)
                .GroupBy(p => p.FechaCreacion.Value.Year)
                .Select(g => new CrecimientoOperacionesDTO
                {
                    id = g.Key,
                    anio = g.Key,
                    operaciones = g.Count(),
                    crecimineto = metasPorAnio.ContainsKey(g.Key)
                        ? ((double)g.Count() / metasPorAnio[g.Key]) * 100  // Operaciones alcanzadas en relación con la meta
                        : 0 // Si el año no tiene meta, crecimiento es 0
                })
                .OrderBy(o => o.anio)
                .ToList();

            var listaCrecimiento = operacionesPorAnio.Select((o, index) => new CrecimientoOperacionesDTO
                                                    {
                                                        id = o.id,
                                                        anio = o.anio,
                                                        operaciones = o.operaciones,
                                                        crecimineto = index == 0 ? 0 :
                                                                      ((double)(o.operaciones - operacionesPorAnio[index - 1].operaciones) /
                                                                      operacionesPorAnio[index - 1].operaciones) * 100
                                                    })
                                                    .ToList();

            return listaCrecimiento;

        }





        [HttpGet("CrecimientoFirmado")]
        public async Task<ActionResult<List<CrecimientoOperacionesDTO>>> CrecimientoFirmado(string? filterValue0, string? operatorValue0, string? columnField0, string genericParam)
        {
            var parametrosArray = genericParam.Split('#');

            var catalogos = _catalogosServicio.ObtenerConsulta().Where(z => z.Tipo == "Año").ToList();



            var anio = parametrosArray[1] == "" ? "" : catalogos.Where(z => z.Id.ToString() == parametrosArray[1]).FirstOrDefault().Valor;
            var mes = parametrosArray[2];
            var broker = parametrosArray[3];
            var ejecutivo = parametrosArray[4];
            var estado = parametrosArray[5];




            var queryBase = _pedidosServicio.ObtenerConsulta()
                .Where(p => p.FechaCreacion.HasValue)
                .Where(z => (anio == "" || z.FechaCreacion.Value.Year.ToString() == anio) &&
                            (mes == "" || z.FechaCreacion.Value.Month.ToString() == mes) &&
                            (broker == "" || z.Broker.ToString() == broker) &&
                            (estado == "" || z.Estado.ToString() == estado))
                .ToList(); // Convertir a lista para evitar múltiples consultas






            // Definir las metas por año
            var metasPorAnio = new Dictionary<int, int>
                                {
                                    { 2020, 15 },
                                    { 2021, 17 },
                                    { 2022, 19 },
                                    { 2023, 21 },
                                    { 2024, 23 },
                                    { 2025, 25 }
                                };

            var operacionesPorAnio = queryBase
                .Where(p => p.FechaCreacion.HasValue)
                .GroupBy(p => p.FechaCreacion.Value.Year)
                .Select(g => new CrecimientoOperacionesDTO
                {
                    id = g.Key,
                    anio = g.Key,
                    operaciones = g.Count(),
                    crecimineto = metasPorAnio.ContainsKey(g.Key)
                        ? ((double)g.Count() / metasPorAnio[g.Key]) * 100  // Operaciones alcanzadas en relación con la meta
                        : 0 // Si el año no tiene meta, crecimiento es 0
                })
                .OrderBy(o => o.anio)
                .ToList();

            var listaCrecimiento = operacionesPorAnio.Select((o, index) => new CrecimientoOperacionesDTO
            {
                id = o.id,
                anio = o.anio,
                operaciones = o.operaciones,
                crecimineto = index == 0 ? 0 :
            ((double)(o.operaciones - operacionesPorAnio[index - 1].operaciones) /
            operacionesPorAnio[index - 1].operaciones) * 100
            }).ToList();

            return listaCrecimiento;

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
