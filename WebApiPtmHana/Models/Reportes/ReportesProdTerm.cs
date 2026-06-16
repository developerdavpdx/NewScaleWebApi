namespace WebApiPtmHana.Models.Reportes
{
    public class ReportesProdTerm
    {
        public int Id_Linea { get; set; }
        public string Codigo { get; set; }
        public int AtadosTarimas { get; set; }
        public int NumTubos { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal PUnitEstandar { get; set; }
        public decimal PUnitReal { get; set; }
        public decimal SobrePeso { get; set; }
        public decimal Scrap { get; set; }
        public decimal Eficiencia { get; set; }
        public decimal ScrapPt { get; set; }
        public decimal ScrapTotal { get; set; }
        public decimal KgPproducto { get; set; }
        public string? Item { get; set; }
        public string? HorasTrabajo { get; set; } //
    }

    public class ReportesDetalle
    {
        public int Id_Linea { get; set; }
        public string? Fechapesaje { get; set; }
        public string? Codigo { get; set; }
        public int AtadosTarimas { get; set; }
        public int NumTubos { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal PUnitEstandar { get; set; }
        public decimal PUnitReal { get; set; }
        public decimal SobrePeso { get; set; }
        public decimal Eficiencia { get; set; }
        //public decimal ScrapTotal { get; set; }
        public decimal KgPproducto { get; set; }
        public decimal HorasTrabajo { get; set; }
    }
}
