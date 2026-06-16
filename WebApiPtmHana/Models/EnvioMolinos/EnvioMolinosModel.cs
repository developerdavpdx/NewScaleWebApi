namespace WebApiPtmHana.Models.EnvioMolinos
{
    public class EnvioMolinosModel
    {
        public string OperadorEM { get; set; }
        public string CodigoItemEM { get; set; }
        public float PesoEM { get; set; }
        public string FamiliaEM { get; set; }
        //public string TipoEM { get; set; }
        //public string SubFamiliaEM { get; set; }
        public int TurnoEM { get; set; }
        public string ComentariosEM { get; set; }
        public string ProcesoEM { get; set; }
        public string UnidadEM { get; set; }
        public string FechaEnvioEM { get; set; }
        public int Linea { get; set; }
        public int planta { get; set; }
    }
}
