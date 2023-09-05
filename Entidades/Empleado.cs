using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Empleado: Entidades
    {
        [Key]
        public int Id { get; set; }
        public Guid IdExterno { get; set; }
        public string Nombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? RFC { get; set; }
        public string? CURP { get; set; }
        public string? Sexo { get; set; }
        public string? CorreoPersonal { get; set; }
        public string? CorreoEmpresarial { get; set; }
        public decimal SueldoDiario { get; set; }
        public decimal SueldoDiarioIntegrado { get; set; }
        public decimal BonoTrimestral { get; set; }
        public int PuestoId { get; set; }
        //public Puesto Puesto { get; set; }
        public string? Sistemas { get; set; }




        //public ICollection<DatoBancarios> DatosBancarios { get; set; }
    }
}
