using API.DTOs;
using Servicios;

namespace API.Auxiliares
{
    public class CatalogAUX: ICatalogAUX
    {
        private readonly IEmpleadosServicio _empleadosServicio;

        public CatalogAUX(IEmpleadosServicio empleadosServicio)
        {
            this._empleadosServicio = empleadosServicio;
        }

        public List<ComboDTO> Combos(string tipo)
        {

            var query = from t in _empleadosServicio.ObtenerConsulta()
                        select new ComboDTO()
                        {
                            id = t.Id.ToString(),
                            Nombre = t.Nombre.ToString()
                        };


            return query.ToList();
        }

    }
}
