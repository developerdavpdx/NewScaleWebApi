using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class HistorialPesadas
    {
        [Key]
        public int Id { get; set; }

        public int? Id_Scrap { get; set; }
        [ForeignKey(nameof(Id_Scrap))]
        public ScrapMolinos ScrapMolinos { get; set; }

        public int? Id_ProductoTerminado { get; set; }
        [ForeignKey(nameof(Id_ProductoTerminado))]
        public ProductoTerminado ProductoTerminado { get; set; }

        public string Tipo { get; set; }

        public string? Comentario { get; set; }
        //public string? ComentarioSAP { get; set; }

        public int? IdEstatusSAP { get; set; }
        [ForeignKey(nameof(IdEstatusSAP))]
        public EstadosSAP EstadosSAP { get; set; }

        public int? IdEstatusHP { get; set; }
        [ForeignKey(nameof(IdEstatusHP))]
        public EstatusHistorialPesadas EstatusHistorialPesadas { get; set; }
        public string? ComentarioSAP { get; set; }
        public string? DocNum { get; set; }
    }
}
