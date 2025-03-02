using API.DTOs;
using ApiBase.DTOs;
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
        public async Task<ActionResult<List<CrecimientoFirmadoDTO>>> CrecimientoFirmado(string? filterValue0, string? operatorValue0, string? columnField0, string genericParam)
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






            // Definir las metas de montos por año
            var metasMontosPorAnio = new Dictionary<int, int>
            {
                { 2020, 150000 },
                { 2021, 170000 },
                { 2022, 190000 },
                { 2023, 210000 },
                { 2024, 230000 },
                { 2025, 250000 }
            };

            var montosPorAnio = _pedidosServicio.ObtenerConsulta()
                .Where(p => p.FechaCreacion.HasValue)
                .GroupBy(p => p.FechaCreacion.Value.Year)
                .Select(g => new CrecimientoFirmadoDTO
                {
                    id = g.Key,
                    anio = g.Key,
                    monto = (double)g.Sum(p => p.Monto), // Suma del monto de pedidos en el año
                    crecimineto = metasMontosPorAnio.ContainsKey(g.Key)
                        ? ((double)g.Sum(p => p.Monto) / metasMontosPorAnio[g.Key]) * 100  // Relación con la meta
                        : 0 // Si no hay meta, crecimiento es 0
                })
                .OrderBy(o => o.anio)
                .ToList();


            return montosPorAnio;

        }


        [HttpGet("DesgloseColocaciones")]
        public async Task<ActionResult<List<DesgloseColocacionDTO>>> DesgloseColocaciones(string? filterValue0, string? operatorValue0, string? columnField0, string genericParam)
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

            var resultados = queryBase.GroupBy(p => p.FechaCreacion.Value.Year)
                                    .Select(g => new DesgloseColocacionDTO
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        anio = g.Key,

                                        // Contar operaciones por tipo
                                        operacionesHipo = g.Count(p => p.Tipo == "51"),
                                        operacionesPyme = g.Count(p => p.Tipo == "52"),
                                        operacionesAuto = g.Count(p => p.Tipo == "53"),
                                        operacionesTotal = g.Count(), // Total de operaciones del año

                                        // Sumar montos de firma por tipo
                                        firmaHipo = (double)g.Where(p => p.Tipo == "51").Sum(p => p.Monto),
                                        firmaPyme = (double)g.Where(p => p.Tipo == "52").Sum(p => p.Monto),
                                        firmaAuto = (double)g.Where(p => p.Tipo == "53").Sum(p => p.Monto),
                                        firmaTotal = (double)g.Sum(p => p.Monto) // Total firmado en el año
                                    })
                                    .OrderBy(x => x.anio)
                                    .ToList();


            return resultados;
        }

        [HttpGet("MontosOperaciones")]
        public async Task<ActionResult<List<MontosOperacionesDTO>>> MontosOperaciones(string? filterValue0, string? operatorValue0, string? columnField0, string genericParam)
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

            var resultados = queryBase.GroupBy(p => p.FechaCreacion.Value.Year)
                                    .Select(g => new MontosOperacionesDTO
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        anio = g.Key,
                                        operacionesTotal = g.Count(), // Total de operaciones del año
                                        firmaTotal = (double)g.Sum(p => p.Monto), // Total firmado en el año
                                        firmaTicket = (double)g.Sum(p => p.Monto)*0.15 // Total firmado en el año
                                    })
                                    .OrderBy(x => x.anio)
                                    .ToList();


            return resultados;
        }











        [HttpGet("ColcoacionEjecutivo")]
        public async Task<ActionResult<List<ColcoacionEjecutivoDTO>>> ColcoacionEjecutivo(string? filterValue0, string? operatorValue0, string? columnField0, string genericParam)
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

            var empleados = ObtenerEmpleadosBrokers();

            var resultados = queryBase
                .GroupBy(p => p.Agente) // Agrupar por ejecutivo (Agente)
                .Select(g => new ColcoacionEjecutivoDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    ejecutivo = empleados.Where(z=>z.BrokerId == g.Key).FirstOrDefault().NombreCompeto, // Método para obtener el nombre del ejecutivo

                    // Contar operaciones por año
                    operaciones2025 = g.Count(p => p.FechaCreacion.Value.Year == 2025),
                    operaciones2024 = g.Count(p => p.FechaCreacion.Value.Year == 2024),
                    operaciones2023 = g.Count(p => p.FechaCreacion.Value.Year == 2023),
                    operaciones2022 = g.Count(p => p.FechaCreacion.Value.Year == 2022),
                    operaciones2021 = g.Count(p => p.FechaCreacion.Value.Year == 2021),
                    operacionesTotal = g.Count(), // Total de operaciones del ejecutivo

                    // Sumar montos de firma por año
                    firma2025 = (double)g.Where(p => p.FechaCreacion.Value.Year == 2025).Sum(p => p.Monto),
                    firma2024 = (double)g.Where(p => p.FechaCreacion.Value.Year == 2024).Sum(p => p.Monto),
                    firma2023 = (double)g.Where(p => p.FechaCreacion.Value.Year == 2023).Sum(p => p.Monto),
                    firma2022 = (double)g.Where(p => p.FechaCreacion.Value.Year == 2022).Sum(p => p.Monto),
                    firma2021 = (double)g.Where(p => p.FechaCreacion.Value.Year == 2021).Sum(p => p.Monto),

                    // Total de firmas considerando todos los años
                    firmaTotal = (double)g.Sum(p => p.Monto)
                })
                .OrderBy(x => x.ejecutivo)
                .ToList();



            return resultados;
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
