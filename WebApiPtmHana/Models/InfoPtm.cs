namespace WebApiPtmHana.Models
{
    public class InfoPtm
    {
        public string CODIGOITEM { get; set; }
        public string NOMBREITEM { get; set; }
        public string UNIDAD { get; set; }
        public double PRECIO { get; set; }
        public double PESOMINIMO {get; set;}
        public double PESOMAXIMO { get; set; }
        public string LINEA { get; set; }
        public double CANTIDAD { get; set; }
        public double COSTO { get; set; }

    }

    public class LineaModel
    {
        public string LINEA { get; set; }
    }

    public class ItemCode
    {
        public string NOMBREITEM { get; set; }
    }
}
