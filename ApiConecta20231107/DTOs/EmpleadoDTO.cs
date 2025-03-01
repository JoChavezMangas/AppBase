using API.Auxiliares;
using Entidades;

namespace API.DTOs
{
    public class EmpleadoDTO: DatosUsuarioDTO
    {

        public Guid idUser { get; set; }
        public int Id { get; set; }
        public string? userName { get; set; }
        public string Nombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string?  ApellidoMaterno { get; set; }

        public string RolId { get; set; }


        public string CURP { get; set; }
        public string? Sexo { get; set; }
        public string CorreoPersonal { get; set; }
        public string CorreoEmpresarial { get; set; }
        


    }


 

}
