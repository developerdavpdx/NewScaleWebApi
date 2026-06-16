using System.ComponentModel.DataAnnotations;

namespace WebApiPtmHana.Datos
{
    public class EstadosSemaforo
    {
        [Key]
        public int Id { get; set; }
        public int Id_Bascula { get; set; }
        public int Estado { get; set; }
    }
}
