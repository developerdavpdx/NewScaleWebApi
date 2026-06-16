namespace WebApiPtmHana.Models.HistorialPesadasModel
{
    public class EditarHitorialPesadas
    {
        public int idHistPesada { get; set; }
        public int idSKU { get; set; }
        public int? LineaPtH { get; set; }
        public float? PesoTotalPtH { get; set; }
        public int? NumTubosPtH { get; set; }
        public float? AtadoPTH { get; set; }
        public float? TaraPTH { get; set; }
        public float? AnilloPTH { get; set; }
        public float? FlejePTH { get; set; }
        public string comentario { get; set; }
        public string nombreTrabjador { get; set; }
    }
}
