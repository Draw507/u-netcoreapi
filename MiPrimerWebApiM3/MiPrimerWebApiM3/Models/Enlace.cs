using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiPrimerWebApiM3.Models
{
    public class Enlace
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Metodo { get; set; }
        public Enlace(string href, string rel, string metodo)
        {
            Href = href;
            Rel = rel;
            Metodo = metodo;
        }
    }
}
