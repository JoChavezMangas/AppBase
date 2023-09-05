using API.Auxiliares;

namespace API.DTOs.OtrosSistemas
{
    public class BrokersMasterDTO : GeneralDTO
    {
        public BrokersMasterDTO() { }
        public BrokersMasterDTO(EmpleadoDTO empleadoDTO) 
        {
            phone = empleadoDTO.telefono;
            email = empleadoDTO.CorreoEmpresarial;
            curp = empleadoDTO.CURP;
            rfc = empleadoDTO.RFC;
            roleID = (Guid)empleadoDTO.empleadoSistemasDTO.brokerRoles; //Guid.Parse("01563f91-f435-49d9-85ed-baae06055df3"),// empleadoDTO.telefonoContacto,
            origin = 1;
            active = empleadoDTO.empleadoSistemasDTO.sistemas[0].Contains(Constantes.OTRO_SISTEMA_BROKERMASTER);
            userName = empleadoDTO.userName;
            Userid = empleadoDTO.idUser;
            firsName = empleadoDTO.Nombre;
            secondName = empleadoDTO.SegundoNombre;
            lastName = empleadoDTO.ApellidoPaterno;
            secondLastName = empleadoDTO.ApellidoMaterno;
        }


        public string phone { get; set; }
        public string curp { get; set; }
        public string rfc { get; set; }
        public string email { get; set; }
        public Guid roleID { get; set; }
    }
}
