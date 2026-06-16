using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class ProductoTerminado
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string CodigoNombre { get; set; }
        public string Clasification { get; set; }
        public int? Id_Linea { get; set; }
        [ForeignKey("Id_Linea")]
        public Lineas Lineas { get; set; }
        public DateTime FechaPesaje { get; set; }
        public float? PesoTotal { get; set; }
        public int? NumTubos { get; set; }
        public int? TurnoPT { get; set; }
        public float? AtadoPT { get; set; }
        public float? TaraPT { get; set; }
        public float? AnilloPT { get; set; }
        public float? FlejePT { get; set; }
        //public float? FolioPT { get; set; }
        public float? FolioPT { get; set; }
        public decimal? KgxHrbyPt { get; set; }
        public bool Calidad { get; set; }
        public string Proceso { get; set; }
        public string OrdenFabricacion { get; set; }
        public int? Planta { get; set; }

        public int? Desactivado { get; set; }
    }
}
