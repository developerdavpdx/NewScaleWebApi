using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.Datos
{
    public class BaseImpresion
    {
        [Key]
        public int Id { get; set; }
        public int IdBascula { get; set; }
        public string Base64 { get; set; }
    }
}
