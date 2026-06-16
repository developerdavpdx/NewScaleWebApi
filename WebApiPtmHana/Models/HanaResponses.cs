using System.Data;

namespace WebApiPtmHana.Models
{
    public class HanaResponses
    {
        public Dictionary<string, string> response { get; set; }
        public DataTable Items { get; set; }
    }

    public class ValidateWeightM
    {
        public string STATUS { get; set; }
        public string ESTADO { get; set; }
    }

    public class NwScaleItem
    {
        public string STATUS { get; set; }
        public string MESSAGE { get; set; }
    }

    public class SkuDetailLinea
    {
        public decimal kgHora { get; set; }
        public int Linea { get; set; }
        public int Planta { get; set; }
        public int Id { get; set; }
        public decimal PesoTotal { get; set; }
    }

    public class LineaData
    {
        public string Sku { get; set; }
        public int Planta { get; set; }
        public decimal KgHora { get; set; }
        public int Linea { get; set; }
        public int Id { get; set; }
        
    }
    public class ExcelKgHora
    {
        public string Codigo { get; set; }
        public decimal KgHora { get; set; }
        public int Linea { get; set; }
        public int Planta { get; set; }

    }
}
