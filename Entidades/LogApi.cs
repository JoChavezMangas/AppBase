using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class LogApi : Entidades
    {
        public int Id { get; set; }
        public string status { get; set; }
        public string urlEnpoint { get; set; }
        public string Message { get; set; }
        public string json { get; set; }
    }
}
