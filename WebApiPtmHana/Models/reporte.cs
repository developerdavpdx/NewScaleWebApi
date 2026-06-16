namespace WebApiPtmHana.Models
{
    public class reporte
    {
        public int Id_Linea { get; set; }
        public string Codigo { get; set; }
        public int AtadosTarimas { get; set; }
        public int NumTubos { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal PesoUnitario { get; set; }
        public decimal SobrePeso { get; set; }
        public decimal Scrap { get; set; }
        public decimal Eficiencia { get; set; }
    }
}
