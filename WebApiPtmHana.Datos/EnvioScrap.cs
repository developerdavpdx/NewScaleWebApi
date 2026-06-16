using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class EnvioScrap
    {
        [Key]
        public int IdEnvioScrap { get; set; }
        public string Operador { get; set; }
        public string CodigoItem { get; set; }
        public double Peso { get; set; }
        public string Familia { get; set; }
        //public string Tipo { get; set; }
        //public string SubFamilia { get; set; }
        public int Turno { get; set; }
        public string Unidad { get; set; }
        public string Proceso { get; set; }
        public string? Comentarios { get; set; }
        public DateTime FechaEnvio { get; set; }
        public int Linea { get; set; }
        public int? ScrapPlanta { get; set; }
        public string? CodigoLinea { get; set; }
    }
}
