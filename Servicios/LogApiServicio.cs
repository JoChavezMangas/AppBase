using Data;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class LogApiServicio : ServiciosBase<LogApi>, ILogApiServicio
    {
        public LogApiServicio(ApiConectaContext context) : base(context, context.LogApi)
        {

        }



    }
}
