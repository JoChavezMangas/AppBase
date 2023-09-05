using API.Auxiliares;

namespace API.DTOs.OtrosSistemas
{
    public class CrediHipotecarioDTO : GeneralDTO
    {
        public CrediHipotecarioDTO() { }
        public CrediHipotecarioDTO(EmpleadoDTO empleadoDTO)
        {
            email = empleadoDTO.CorreoEmpresarial;
            curp = empleadoDTO.CURP;
            bankID = (Guid)empleadoDTO.empleadoSistemasDTO.crediBancos;// Guid.Parse("36e14ca1-d309-4022-98f6-5dae2c0a98cc"),
            roleID = (Guid)empleadoDTO.empleadoSistemasDTO.crediRoles;// Guid.Parse("33788b92-bb3d-409a-9235-ea33243d0e86"),
            origin = 1;
            active = empleadoDTO.empleadoSistemasDTO.sistemas[0].Contains(Constantes.OTRO_SISTEMA_CREDIHIPO);
            userName = empleadoDTO.userName;
            Userid = empleadoDTO.idUser;
            firsName = empleadoDTO.Nombre;
            secondName = empleadoDTO.SegundoNombre;
            lastName = empleadoDTO.ApellidoPaterno;
            secondLastName = empleadoDTO.ApellidoMaterno;
        }

        public string curp { get; set; }
        public string email { get; set; }
        public Guid bankID { get; set; }
        public Guid roleID { get; set; }
    }
}
