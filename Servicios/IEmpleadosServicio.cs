using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public interface IEmpleadosServicio
    {
        Task<List<Empleado>> ObtenerListaAsync();
        void Crear(Empleado model,string _contextEmpleadoId);
        void Actualizar(Empleado model, string _contextEmpleadoId);
        IQueryable<Empleado> ObtenerConsulta();
        void Borrar(object id, string _contextEmpleadoId);
    }
}
