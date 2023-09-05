namespace API.DTOs
{
    public class RespuestaAutenticacion
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
        public string EmpleadoId { get; set; }
        public string RolId { get; set; }
    }
}
