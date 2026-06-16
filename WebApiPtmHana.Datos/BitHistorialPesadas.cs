using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class BitHistorialPesadas
    {
        [Key]
        public int Id { get; set; }
        public int IdHistorialPesada { get; set; }
        [ForeignKey(nameof(IdHistorialPesada))]
        public HistorialPesadas HistorialPesadas { get; set; }
        public string Codigo { get; set; }
        public string CodigoNombre { get; set; }
        public int? LineaPtH { get; set; }
        public float? PesoTotalPtH { get; set; }
        public int? NumTubosPtH { get; set; }
        public float? AtadoPTH { get; set; }
        public float? TaraPTH { get; set; }
        public float? AnilloPTH { get; set; }
        public float? FlejePTH { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
