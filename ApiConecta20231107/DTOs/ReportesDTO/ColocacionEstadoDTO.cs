namespace ApiBase.DTOs.ReportesDTO
{
    public class ColocacionEstadoDTO
    {
        public int estado { get; set; }
        public int anio { get; set; }
        public double porcentage { get; set; }
        public int operaciones { get; set; }
        public double total { get; set; }
    }

    public class ReporteColocacionEstadoDTO
    {
        public string id { get; set; }
        public string estado { get; set; }
        public string a2025 { get; set; }
        public string a2024 { get; set; }
        public string a2023 { get; set; }
        public string a2022 { get; set; }
        public string a2021 { get; set; }
        public string a2020 { get; set; }
        public string total { get; set; }
    }

}
