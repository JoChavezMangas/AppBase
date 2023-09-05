using Entidades;

namespace API.DTOs
{
    public class EmpresaDTO: DatosUsuarioDTO
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; }
        public string RazonComercial { get; set; }
        public string RepresentanteLegal { get; set; }
        public string RFC { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string CodigoPostal { get; set; }
        public string Direccion { get; set; }
        public byte Logo { get; set; }
        public int TipoEmpresaId { get; set; }
        public IFormFile? archivo { get; set; }

        public string RazonSocialFiltro {get{ return RazonSocial.Replace(" ", ""); } }


    }
}
