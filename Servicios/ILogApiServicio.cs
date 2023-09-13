using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public interface ILogApiServicio
    {
        void Crear(LogApi model, string _currentUser);

    }
}
