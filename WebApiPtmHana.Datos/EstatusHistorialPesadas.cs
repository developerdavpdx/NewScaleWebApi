using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class EstatusHistorialPesadas
    {
        [Key]
        public int Id { get; set; }
        public string EstatusColor { get; set; }
        public string DescripcionEstatus { get; set; }
    }
}
