using API.Auxiliares;

namespace API.DTOs.OtrosSistemas
{
    public class ValidocDTO:GeneralDTO
    {
        public ValidocDTO() { }
        public ValidocDTO(EmpleadoDTO empleadoDTO) 
        {
            var listaOperacion = empleadoDTO.empleadoSistemasDTO.validocOperacion.Split(',').Select(Guid.Parse).ToList();
            email = empleadoDTO.CorreoEmpresarial;
            curp = empleadoDTO.CURP;
            supervisorID = Guid.Empty;
            typeOperation = listaOperacion;
            roleID = (Guid)empleadoDTO.empleadoSistemasDTO.validocRoles; //Guid.Parse("6917a16e-4845-4413-ab15-2291afad5a48"),
            origin = 1;
            active = empleadoDTO.empleadoSistemasDTO.sistemas[0].Contains(Constantes.OTRO_SISTEMA_VALIDOC);
            userName = empleadoDTO.userName;
            Userid = empleadoDTO.idUser;
            firsName = empleadoDTO.Nombre;
            secondName = empleadoDTO.SegundoNombre;
            lastName = empleadoDTO.ApellidoPaterno;
            secondLastName = empleadoDTO.ApellidoMaterno;
        }
        //public int TipoDeOperación { get; set; }
        public string email { get; set; }
        public string curp { get; set; }
        public Guid supervisorID { get; set; }
        public Guid roleID { get; set; }
        public List<Guid> typeOperation { get; set; }

    }
}
