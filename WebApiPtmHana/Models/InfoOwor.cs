namespace WebApiPtmHana.Models
{
    public class InfoOwor
    {
        public string CODIGOITEM { get; set; }
        public string NOMBRE { get; set; }
        public string NOMBREITEM { get; set; }
        public  string CATEGORIA { get; set; }
        public double PESOMINIMO { get; set; }
        public double PESOMAXIMO { get; set; }
        public string LINEA { get; set; }
    }

    public class BaseEntryInfo
    {
        public int BASENTRY { get; set; }
        public int DOCNUM { get; set; }
    }
}
