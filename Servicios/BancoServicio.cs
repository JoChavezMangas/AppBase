using Data;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class BancoServicio : ServiciosBase<Banco>, IBancoServicio
    {
        public BancoServicio(ApiConectaContext context) : base(context, context.Banco)
        {

        }

    }
}
