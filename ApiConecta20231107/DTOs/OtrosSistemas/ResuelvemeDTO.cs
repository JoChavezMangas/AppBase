using API.Auxiliares;
using API.DTOs;
using API.DTOs.OtrosSistemas;

namespace ApiConecta20231107.DTOs.OtrosSistemas
{
    public class ResuelvemeDTO : GeneralDTO
    {
        public ResuelvemeDTO(){ }


        public ResuelvemeDTO(EmpleadoDTO empleadoDTO) 
        {
            if (empleadoDTO.empleadoSistemasDTO == null)
                empleadoDTO.empleadoSistemasDTO = new EmpleadoSistemasDTO();

            email = empleadoDTO.CorreoEmpresarial;
            phone = empleadoDTO.telefono;
            companyId = (Guid)empleadoDTO.empleadoSistemasDTO.resuelveEmpresa;
            departamentId = (Guid)empleadoDTO.empleadoSistemasDTO.resuleveDepartameto;
            roleId = (Guid)empleadoDTO.empleadoSistemasDTO.resuelveRoles; //Guid.Parse("06b60c0e-5dc1-4037-b707-d9584793fce4"),
            origin = 1;
            active = empleadoDTO.empleadoSistemasDTO.sistemas[0].Contains(Constantes.OTRO_SISTEMA_RESUELVEME);
            userName = empleadoDTO.userName;
            Userid = empleadoDTO.idUser;
            firsName = empleadoDTO.Nombre;
            secondName = empleadoDTO.SegundoNombre;
            lastName = empleadoDTO.ApellidoPaterno;
            secondLastName = empleadoDTO.ApellidoMaterno;
        }

        public string email { get; set; }
        public string phone { get; set; }
        public Guid roleId { get; set; }
        public Guid companyId { get; set; }
        public Guid departamentId { get; set; }
        public int origin { get; set; }
    }
}
