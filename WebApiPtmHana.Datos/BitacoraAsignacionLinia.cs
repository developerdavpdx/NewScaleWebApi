using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class BitacoraAsignacionLinia
    {
        [Key]
        public int Id { get; set; }
        public int Linea { get; set; }
        public string Trabajador { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string CodigoItem { get; set; }
        public string Planta { get; set; }
    }
}
