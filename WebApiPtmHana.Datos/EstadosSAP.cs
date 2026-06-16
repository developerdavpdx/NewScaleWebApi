using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class EstadosSAP
    {
        [Key]
        public int Id { get; set; }
        public string Estatus { get; set; }
        public string Descripcion { get; set; }
    }
}
