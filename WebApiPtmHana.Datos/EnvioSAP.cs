using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class EnvioSAP
    {
        [Key]
        public int Id { get; set; }
        public int IdentEnvioSAP { get; set; }
        public int IdHistPesada { get; set; }
        [ForeignKey(nameof(IdHistPesada))]
        public HistorialPesadas HistorialPesadas { get; set; }
        public int IsPrelim { get; set; }
    }
}
