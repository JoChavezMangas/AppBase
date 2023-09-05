using Data;
using Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class ServiciosBase<T> where T : Entidades.Entidades
    {
        protected readonly ApiConectaContext _context;
        protected DbSet<T> _servicio;
        //protected Entidades.Entidades _entidades;

        public ServiciosBase(ApiConectaContext context, DbSet<T> servicio)
        {
            _context = context;
            _servicio = servicio;
        }

        public async void Crear(T obj, string _contextEmpleadoId)
        {
            obj.FechaCreacion = DateTime.Now;
            obj.CreadoPor = _contextEmpleadoId;
            obj.FechaModificacion = DateTime.Now;
            obj.ModificadoPor = _contextEmpleadoId;
            _servicio.Add(obj);
            _context.SaveChanges();
        }

        public IQueryable<T> ObtenerConsulta()
        {
            var query = _servicio.AsQueryable().Where(z=>!z.EsBorrado);
            return query;
        }

        public async Task<List<T>> ObtenerListaAsync()
        {
            var query =_servicio.ToListAsync();
            return await query;
        }

        public void Actualizar(T obj, string _contextEmpleadoId)
        {
            obj.FechaModificacion = DateTime.Now;
            obj.ModificadoPor = _contextEmpleadoId;
            _servicio.Update(obj);
            _context.SaveChanges();
        }

        public void Borrar (object id,string _contextEmpleadoId)
        {
            var obj = _servicio.FindAsync(id).Result;
            if (obj != null)
            {
                obj.FechaModificacion = DateTime.Now;
                obj.ModificadoPor = _contextEmpleadoId;
                obj.EsBorrado = true;
                _servicio.Update(obj);
                _context.SaveChanges();
            }
        }
    }
}
