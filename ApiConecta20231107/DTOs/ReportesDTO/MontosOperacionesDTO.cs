namespace ApiBase.DTOs.ReportesDTO
{
    public class MontosOperacionesDTO
    {
        public string Id { get; set; }
        public int anio { get; set; }

        public int operacionesTotal { get; set; }
        public double firmaTotal { get; set; }
        public double firmaTicket { get; set; }
    }
}
