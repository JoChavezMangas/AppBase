using API.Auxiliares;
using API.DTOs;
using API.DTOs.OtrosSistemas;

namespace ApiConecta20231107.DTOs.OtrosSistemas
{
    public class SisecDTO : GeneralDTO
    {
        public SisecDTO() { }
        public SisecDTO (EmpleadoDTO empleadoDTO)
        {
            var farnchiseArray = empleadoDTO.empleadoSistemasDTO.sisecFranquicia;
            var farnchiseList = !string.IsNullOrEmpty(farnchiseArray) ? farnchiseArray.Split(',').Select(Guid.Parse).ToList() : new List<Guid>();

            phone = empleadoDTO.telefono;
            curp = empleadoDTO.CURP;
            email = empleadoDTO.CorreoEmpresarial;
            franchise = farnchiseList;
            roleID = (Guid)(empleadoDTO.empleadoSistemasDTO.sisecRoles ?? Guid.Empty);//Guid.Parse("23cfd180-2225-44b2-9be4-c665d7d45350")
            Userid = empleadoDTO.idUser;
            origin = 1;
            active = empleadoDTO.empleadoSistemasDTO.sistemas[0].Contains(Constantes.OTRO_SISTEMA_SISEC);
            userName = empleadoDTO.userName;
            Userid = empleadoDTO.idUser;
            firsName = empleadoDTO.Nombre;
            secondName = empleadoDTO.SegundoNombre;
            lastName = empleadoDTO.ApellidoPaterno;
            secondLastName = empleadoDTO.ApellidoMaterno;
        }


        public string phone { get; set; }
        public string curp { get; set; }
        public string email { get; set; }
        public List<Guid> franchise { get; set; }
        //public string userName { get; set; }
        public Guid roleID { get; set; }
        public int origin { get; set; }
    }
}
