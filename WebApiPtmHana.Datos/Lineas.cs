using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class Lineas
    {
        [Key]
        public int Id_linea { get; set; }
        public string Descripcion { get; set; }
        public bool Asignado { get; set; }
        public string Proceso { get; set; }
        public decimal KgXHr { get; set; }
        public string? ItemCodeLinea { get; set; }
        public string? OrdenFabricacion { get; set; }
        public string? FolioSAP { get; set; }
        public int? Planta { get; set; }
        public int? NumLinea { get; set; }
    }
}
