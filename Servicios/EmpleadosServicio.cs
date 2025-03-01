using Data;
using Datos;
using Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
namespace Servicios
{
    public class EmpleadosServicio : ServiciosBase<Empleado>, IEmpleadosServicio
    {
        public EmpleadosServicio(ApiConectaContext context) : base(context, context.Empleados)
        {

        }

        public Empleado ObtenerEmpleado(int Id)
        {
            var query = _servicio.Find(Id);
            return query;
        }


    }
}
