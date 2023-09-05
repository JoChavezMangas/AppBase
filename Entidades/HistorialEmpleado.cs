using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class HistorialEmpleado:Entidades
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public string NuevoCambio { get; set; }
        public string DatoAnterior { get; set; }
        public string Identificador { get; set; }

    }
}
