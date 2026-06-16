using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class AcumuladoEmbarque
    {
        [Key]
        public int id { get; set; }
        public string Proceso { get; set; }
        public string Familia { get; set; }
        public float Acumulado { get; set; }
        public DateTime FechaIngreso { get; set; }
        public int Planta { get; set; }
    }
}
