using API.Auxiliares;
using Entidades;

namespace API.DTOs
{
    public class EmpleadoDTO: DatosUsuarioDTO
    {

        public Guid idUser { get; set; }
        public int Id { get; set; }
        public string? userName { get; set; }
        public string Nombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string?   ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        //public string FechaNacimientoString { get; set; }
        
        public string CURP { get; set; }
        public string? Sexo { get; set; }
        public string CorreoPersonal { get; set; }
        public string CorreoEmpresarial { get; set; }
        

        public int PuestoId { get; set; }
        public int AreaId { get; set; }
        public int EmpresaId { get; set; }
        public string RolId { get; set; }
        public string? NombreEmpresa { get; set; }
        public string? NombreArea { get; set; }
        //public string NombreDepartamento { get; set; }
        public string? NombrePuesto { get; set; }
        public string? NombreCompleto { get; set; }


        //EmpleadoDireccion
        public string? pais { get; set; }
        public string? estado { get; set; }
        public string? municipio { get; set; }
        public string? colonia { get; set; }
        public string? calle { get; set; }
        public string? codigoPostal { get; set; }
        public string? telefonoFijo { get; set; }


        //Tipo Contratacion
        public string RFC { get; set; }
        public string? nss { get; set; }
        public string telefono { get; set; }
        public DateTime ? fechaIngreso { get; set; }
        public DateTime  fechaTerminacionPrueba { get; set; }
        public string modalidad { get; set; }


        // Datos Bancarios
        public string NoCuenta { get; set; }
        public string? clabe { get; set; }
        public int bancoId { get; set; }
        public Guid? bancoExterno { get; set; }
        public string? numeroTarjeta { get; set; }


        // Datos Nomina
        public decimal SueldoDiario { get; set; }
        public decimal SueldoDiarioIntegrado { get; set; }
        public decimal BonoTrimestral { get; set; }
        public DateTime fechaSueldo { get; set; }
        public string? tipoContrato { get; set; }
        public string? duracionContrato { get; set; }
        public DateTime? finContrato { get; set; }
        public string? tipoRegimen { get; set; }
        public string? razonSocial { get; set; }
        public bool esquemaOutsourcing { get; set; }


        //EmpleadoContactoEmergecia
        public string? nombreContacto { get; set; }
        public string? parentesco { get; set; }
        public string? celularContacto { get; set; }
        public string? telefonoContacto { get; set; }
        public string? correoContacto { get; set; }

        public int? liderDirecto { get; set; }

        public string[] sistemas { get; set; }
        public EmpleadoSistemasDTO? empleadoSistemasDTO { get; set; }



    }

    public class EmpleadoSistemasDTO: DatosUsuarioDTO
    {


        public int id { get; set; }
        public string[] sistemas { get; set; }
        public int? agenteRoles { get; set; }   
        public Guid? brokerRoles { get; set; }
        public Guid? cvelozRoles { get; set; }
        public Guid? crediRoles { get; set; }
        public Guid? crediBancos { get; set; }
        public Guid? sisecRoles { get; set; }
        public Guid? resuelveRoles { get; set; }
        public Guid? validocRoles { get; set; }
        public string? cvelozBancos { get; set; }
        public string? validocOperacion { get; set; }
        public string? cvelozAnios { get; set; }
        public Guid? sicafiRoles { get; set; }
        public Guid? resuelveEmpresa { get; set; }
        public Guid? resuleveDepartameto { get; set; }
        public string? sicafiFranquicia { get; set; }
        public string? sisecFranquicia { get; set; }
    }

 

}
