using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Banco : Entidades
    {
        [Key]
        public int Id { get; set; }
        public Guid IdExterno { get; set; }
        public string Nombre { get; set; }
        
    }
}
