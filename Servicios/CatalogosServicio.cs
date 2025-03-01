using Data;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class CatalogosServicio : ServiciosBase<Catalogos>, ICatalogosServicio
    {
        public CatalogosServicio(ApiConectaContext context) : base(context, context.Catalogos)
        {

        }
    }
}
