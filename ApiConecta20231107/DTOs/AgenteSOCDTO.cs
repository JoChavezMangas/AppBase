using API.Auxiliares;
using API.DTOs.OtrosSistemas;

namespace API.DTOs
{
    public class AgenteSOCDTO:GeneralDTO
    {
        public AgenteSOCDTO(){ }

        public AgenteSOCDTO(EmpleadoDTO empleadoDTO) 
        {
            email = empleadoDTO.CorreoEmpresarial;
            rfc = empleadoDTO.RFC;
            phone = empleadoDTO.telefono;
            roleID = (int)empleadoDTO.empleadoSistemasDTO.agenteRoles;
            origin = 1;
            active = empleadoDTO.empleadoSistemasDTO.sistemas[0].Contains(Constantes.OTRO_SISTEMA_AGENTESOC);
            userName = empleadoDTO.userName;
            Userid = empleadoDTO.idUser;
            userID = empleadoDTO.idUser;
            firsName = empleadoDTO.Nombre;
            secondName = empleadoDTO.SegundoNombre;
            lastName = empleadoDTO.ApellidoPaterno;
            secondLastName = empleadoDTO.ApellidoMaterno;
            
        }


        public Guid userID { get; set; }
        public string email { get; set; }
        public string rfc { get; set; }
        public string phone { get; set; }
        //public string Telefono { get; set; }
        public int roleID { get; set; }
    }
}
