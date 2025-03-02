namespace ApiBase.DTOs.ReportesDTO
{
    public class DesgloseColocacionDTO
    {
        public string Id { get; set; }
        public int anio { get; set; }
        public int operacionesHipo { get; set; }
        public int operacionesPyme { get; set; }
        public int operacionesAuto { get; set; }
        public int operacionesTotal => operacionesHipo + operacionesPyme + operacionesAuto;
        public double firmaHipo { get; set; }
        public double firmaPyme { get; set; }
        public double firmaAuto { get; set; }
        public double firmaTotal => firmaHipo + firmaPyme + firmaAuto;


    }
}
