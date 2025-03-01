using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public interface IBrokersServicio
    {
        Task<List<Brokers>> ObtenerListaAsync();
        void Crear(Brokers model, string _contextEmpleadoId);
        void Actualizar(Brokers model, string _contextEmpleadoId);
        IQueryable<Brokers> ObtenerConsulta();
        void Borrar(object id, string _contextEmpleadoId);
    }
}
