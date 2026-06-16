using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class SkuInfo
    {
        [Key]
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public float? MultiploCant { get; set; }
        public string? MultiploCategoria { get; set; }
        public float Variable { get; set; }
        public string? FijoCategoria { get; set; }
        public string? Proceso { get; set; }
        public float? FijoCantidad { get; set; }
        public int? NumTubos { get; set; }
        public DateTime LastUpdate { get; set; }
        public string? Usuario { get; set; }
        public float? MinWeight { get; set; }
        public float? AnilloCant { get; set; }
        public float? CostalesCant { get; set; }
        public float? ArpillaCant { get; set; }
        public float? TarimaCant { get; set; }
        public float? FlejeCant { get; set; }
        public float? MaderaCant { get; set; }
        public int? Planta { get; set; }
    }
}
