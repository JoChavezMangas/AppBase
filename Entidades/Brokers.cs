using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Brokers : Entidades
    {
        [Key]
        public int Id { get; set; }
        public Guid IdExterno { get; set; }
        public string NombreComercial { get; set; }
        public string RazonSocial { get; set; }
        public string? RFC { get; set; }

    }
}
