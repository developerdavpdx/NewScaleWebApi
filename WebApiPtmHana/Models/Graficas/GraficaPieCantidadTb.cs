namespace WebApiPtmHana.Models.Graficas
{
    public class GraficaPieCantidadTb
    {
        public int Linea { get; set; }
        public int Turno { get; set; }
        public int CantidadPt { get; set; }
        public int CantidadE { get; set; }
    }

    public class GraficaSobrePeso
    {
        public int Id_SKU { get; set; }
        public int Id_Linea { get; set; }
        public float TaraTotal { get; set; }
        public float PesoNetoTotal { get; set; }
        public float PesoEstandarTotal { get; set; }
        public int NumTubos { get; set; }
        public float PesoMinimo { get; set; }
        public float sobrePeso { get; set; }
        public float KgxHrbyPt { get; set; }
    }

    public class EficienciaInfo
    {
        public float PesoScrap { get; set; }
        public float PesoPT { get; set; }
        public float KGxHR { get; set; }
    }

    public class GraficaLineasSW
    {
        public int? Id_Linea { get; set; }
        public float? PesoTotal { get; set; } 
        public float? PesoEstandar { get; set; }
        public float? SobrePeso { get; set; }
    }

    public class KPISinfo
    {
        public float Linea { get; set; }
        public float Volumen { get; set; }
        public string? Codigos { get; set; }
        public float Sobrepeso { get; set; }
        public float SobrepesoObjetivo { get; set; }
        public float ScrapKG { get; set; }
        public float ScrapPorcent { get; set; }
        public float ScrapObjetivo { get; set; }
        public float Eficiencia { get; set; }
    }
}