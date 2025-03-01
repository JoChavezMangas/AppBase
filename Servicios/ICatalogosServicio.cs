using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public interface ICatalogosServicio
    {
        Task<List<Catalogos>> ObtenerListaAsync();
        void Crear(Catalogos model, string _contextEmpleadoId);
        void Actualizar(Catalogos model, string _contextEmpleadoId);
        IQueryable<Catalogos> ObtenerConsulta();
        void Borrar(object id, string _contextEmpleadoId);
    }
}
