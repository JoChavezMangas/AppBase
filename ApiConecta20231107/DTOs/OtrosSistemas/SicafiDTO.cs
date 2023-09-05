//using static System.Runtime.InteropServices.JavaScript.JSType;

using API.Auxiliares;

namespace API.DTOs.OtrosSistemas
{
    public class SicafiDTO : GeneralDTO
    {
        public SicafiDTO() { }
        public SicafiDTO(EmpleadoDTO empleadoDTO) 
        {
            var franchiseList = empleadoDTO.empleadoSistemasDTO.sicafiFranquicia.Split(',').Select(Guid.Parse).ToList();
            roleID = (Guid)empleadoDTO.empleadoSistemasDTO.sicafiRoles;
            email = empleadoDTO.CorreoEmpresarial;
            franchise = franchiseList;
            origin = 1;
            active = empleadoDTO.empleadoSistemasDTO.sistemas[0].Contains(Constantes.OTRO_SISTEMA_SICAFI);
            userName = empleadoDTO.userName;
            Userid = empleadoDTO.idUser;
            firsName = empleadoDTO.Nombre;
            secondName = empleadoDTO.SegundoNombre;
            lastName = empleadoDTO.ApellidoPaterno;
            secondLastName = empleadoDTO.ApellidoMaterno;
        }
        public Guid roleID { get; set; }
        public string email { get; set; }
        public List<Guid> franchise { get; set; }
        public int origin { get; set; }
    }
}
