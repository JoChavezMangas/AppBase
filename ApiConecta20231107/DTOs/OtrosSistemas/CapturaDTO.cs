using API.Auxiliares;
using API.DTOs;
using API.DTOs.OtrosSistemas;

namespace ApiBase.DTOs.OtrosSistemas
{
    public class CapturaDTO : GeneralDTO
    {
        public CapturaDTO() { }
        public CapturaDTO(EmpleadoDTO empleadoDTO) 
        {
            var anios = empleadoDTO.empleadoSistemasDTO.cvelozAnios.Split(',').Select(int.Parse).ToList();
            var bancos = empleadoDTO.empleadoSistemasDTO.cvelozBancos.Split(',').Select(int.Parse).ToList();
            roleID = (Guid)empleadoDTO.empleadoSistemasDTO.cvelozRoles;// Guid.Parse("a1123e0b-5013-47ca-a4f9-725f0721e2d3"),
            email = empleadoDTO.CorreoEmpresarial;
            banks = bancos;
            years = anios;
            origin = 1;
            active = empleadoDTO.empleadoSistemasDTO.sistemas[0].Contains(Constantes.OTRO_SISTEMA_CAPTURA);
            userName = empleadoDTO.userName;
            Userid = empleadoDTO.idUser;
            firsName = empleadoDTO.Nombre;
            secondName = empleadoDTO.SegundoNombre;
            lastName = empleadoDTO.ApellidoPaterno;
            secondLastName = empleadoDTO.ApellidoMaterno;
        }
        public Guid roleID { get; set; }
        public string email { get; set; }
        public List<int> banks { get; set; }
        public List<int> years { get; set; }
        public int origin { get; set; }
}
}
