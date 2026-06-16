namespace WebApiPtmHana.Models.Scrap
{
    public class Scrap
    {
        public int? IdEnvioScrap { get; set; }
        public string OperadorScrap { get; set; }
        public string CodigoItemScrap { get; set; }
        public float PesoScrap { get; set; }
        public string FamiliaScrap { get; set; }
        public string TipoScrap { get; set; }
        public string SubFamiliaScrap { get; set; }
        public string EstadoScrap { get; set; }
        public int TurnoScrap { get; set; }
        public string ProcesoScrap { get; set; }
        public string UnidadScrap { get; set; }
        public int IdRemolidoScrap { get; set; }
        public int planta { get; set; }
    }
}
