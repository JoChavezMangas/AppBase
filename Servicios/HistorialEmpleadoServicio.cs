using Datos;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Servicios
{
    public class HistorialEmpleadoServicio :ServiciosBase<HistorialEmpleado>, IHistorialEmpleadoServicio
    {

        public HistorialEmpleadoServicio(ApiConectaContext context) : base(context, context.HistorialEmpleado)
        {

        }

        public List<HistorialEmpleado> ObtenerLista()
        {
            var query = _servicio.ToList();
            return query;
        }

    }
}
