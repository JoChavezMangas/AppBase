using Data;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class BrokersServicio: ServiciosBase<Brokers>, IBrokersServicio
    {
        public BrokersServicio(ApiConectaContext context) : base(context, context.Brokers)
        {

        }

    }
}
