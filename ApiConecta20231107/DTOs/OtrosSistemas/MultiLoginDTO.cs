using API.Auxiliares;

namespace API.DTOs.OtrosSistemas
{
    public class MultiLoginDTO:GeneralDTO
    {
        public MultiLoginDTO() { }
        public MultiLoginDTO(EmpleadoDTO empleadoDTO) 
        {
            string nombreCompleto = empleadoDTO.Nombre + " " +
                                            (string.IsNullOrEmpty(empleadoDTO.SegundoNombre) ? empleadoDTO.SegundoNombre + " " : "") +
                                            empleadoDTO.ApellidoPaterno + " " +
                                            empleadoDTO.ApellidoMaterno;

            User = new User();
            User.Email = empleadoDTO.CorreoEmpresarial;
            User.PhoneNumber = empleadoDTO.telefono;
            User.Password = "n3wPa$$w0rdT3st";
            User.UserType = "Empleado Interno";

            Claims = new Dictionary<string, string>();
            Claims.Add("name", nombreCompleto);
            Claims.Add("CURP", empleadoDTO.CURP);
            Claims.Add("userIdClient", empleadoDTO.idUser.ToString());
            Claims.Add("company", "GrupoApa");

            userName = empleadoDTO.userName;
            Userid = empleadoDTO.idUser;
            firsName = empleadoDTO.Nombre;
            secondName = empleadoDTO.SegundoNombre;
            lastName = empleadoDTO.ApellidoPaterno;
            secondLastName = empleadoDTO.ApellidoMaterno;
        }


        public User User { get; set; }
        public Dictionary<string, string> Claims { get; set; }
    }


    public class User : GeneralDTO
    {
        public User() { }
        public User(EmpleadoDTO empleadoDTO) 
        {
            UserId = empleadoDTO.idUser.ToString();
            Userid = empleadoDTO.idUser;
            UserName = empleadoDTO.userName;
            userName = empleadoDTO.userName;
            IsActive = true;
            active = true;
        }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
    }

}
