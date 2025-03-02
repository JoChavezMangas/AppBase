using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Pedidos : Entidades
    {
        [Key]
        public int Id { get; set; }
        public Guid IdExterno { get; set; }
        public int Broker { get; set; }
        public int Estado { get; set; }
        public decimal Monto { get; set; }
        public Guid Agente { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int Banco { get; set; }
        public string Tipo { get; set; }
    }
}
