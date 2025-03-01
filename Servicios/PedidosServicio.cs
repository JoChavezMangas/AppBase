using Data;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class PedidosServicio : ServiciosBase<Pedidos>, IPedidosServicio
    {
        public PedidosServicio(ApiConectaContext context) : base(context, context.Pedidos)
        {

        }
    }
}
