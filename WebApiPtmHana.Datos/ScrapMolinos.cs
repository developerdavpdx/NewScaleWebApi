using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class ScrapMolinos
    {
        [Key]
        public int IdScrap { get; set; }
        public int? IdEnvioScrap { get; set; }
        [ForeignKey(nameof(IdEnvioScrap))]
        public EnvioScrap EnvioScrap { get; set; }
        public string Operador { get; set; }
        public string CodigoItem { get; set; }
        public double Peso { get; set; }
        public string Familia { get; set; }
        public string Tipo { get; set; }
        public string SubFamilia { get; set; }
        public string Estado { get; set; }
        public int Turno { get; set; }
        public string Unidad { get; set; }
        public string Proceso { get; set; }
        public DateTime FechaScrap { get; set; }
        public int IdRemolido { get; set; }
        public int Cantidad { get; set; }
        public float AtadoScrpt { get; set; }
        public float TaraScrpt { get; set; }
        public float AnilloScrpt { get; set; }
        public float FlejeScrpt { get; set; }
        public int? PlantaScrpt { get; set; }
    }
}
