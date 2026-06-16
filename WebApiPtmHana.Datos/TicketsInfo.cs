using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class TicketsInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTicket { get; set; }
        public string Folio { get; set; }
        public string OrdenFabricacion { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int Turno { get; set; }
        public int Id_Linea { get; set; }
        [ForeignKey("Id_Linea")]
        public Lineas Linea { get; set; }
        public int Operacion { get; set; }
        public int? NumTubos { get; set; }
        public float? PesoNeto { get; set; }
        public float? PesoBascula { get; set; }
        public string? Medida { get; set; }
        public float? Porcentaje { get; set; }
        public string Categoria { get; set; }
        public decimal PesoTubo { get; set; }
        public DateTime? FechaPesaje { get; set; }
        public bool AsignadoTicket { get; set; }
        public int? PlantaTicket { get; set; }
    }
}
