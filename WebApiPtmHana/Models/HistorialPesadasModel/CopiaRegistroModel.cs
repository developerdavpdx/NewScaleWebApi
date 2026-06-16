namespace WebApiPtmHana.Models.HistorialPesadasModel
{
    public class CopiaRegistroModel
    {
        public string idHPesado { get; set; }
        public string comentario { get; set; }
        public string Codigo { get; set; }
        public int Id_Linea { get; set; }
        public float PesoTotal { get; set; }
        public int NumTubos { get; set; }
        public float AtadoPT { get; set; }
        public float TaraPT { get; set; }
        public float AnilloPT { get; set; }
        public float FlejePT { get; set; }
    }
}
