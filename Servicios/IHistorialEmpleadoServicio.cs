using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public interface IHistorialEmpleadoServicio
    {
        List<HistorialEmpleado> ObtenerLista();
        void Crear(HistorialEmpleado modelo, string _contextEmpleadoId);
    }
}
