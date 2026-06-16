using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class EstadosPesos
    {
        [Key]
        public int Id { get; set; }
        public int Id_Bascula { get; set; }
        public int PesoBascula { get; set; }
    }
}
