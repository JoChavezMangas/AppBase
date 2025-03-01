using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public interface IPedidosServicio
    {
        Task<List<Pedidos>> ObtenerListaAsync();
        void Crear(Pedidos model, string _contextEmpleadoId);
        void Actualizar(Pedidos model, string _contextEmpleadoId);
        IQueryable<Pedidos> ObtenerConsulta();
        void Borrar(object id, string _contextEmpleadoId);
    }
}
