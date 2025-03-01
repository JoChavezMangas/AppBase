using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public interface IBancoServicio
    {
        Task<List<Banco>> ObtenerListaAsync();
        void Crear(Banco model, string _contextEmpleadoId);
        void Actualizar(Banco model, string _contextEmpleadoId);
        IQueryable<Banco> ObtenerConsulta();
        void Borrar(object id, string _contextEmpleadoId);
    }
}
