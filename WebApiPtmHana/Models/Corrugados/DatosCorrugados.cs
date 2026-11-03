using System.Numerics;
using System.Reflection.Metadata;

namespace WebApiPtmHana.Models.DatosCorrugados
{
    public class DataAsignarLinea   //recibe los datos para asignar una linea
    {
        public string? code { get; set; }
        public int linea { get; set; }
        public int turno { get; set; }
        public decimal kgxhora { get; set; }
        public int ordenfabricacion { get; set; }
        public int folio { get; set; }
        public int planta { get; set; }
        public string? NameUser { get; set; }
    }

    public class DataGeneralLineaPlanta   //recibe de linea y planta
    {
        public int linea { get; set; }
        public int planta { get; set; }
    }

    public class DataPesosObtenidos   //guarda los datos recibidos para hacer los calculos de los pesos
    {
        public int NumTubos { get; set; }
        public double Variable { get; set; }
        public double AnilloCantidad { get; set; }
        public double FlejeCantidad { get; set; }
        public double ArpillaCantidad { get; set; }
        public double TarimaCantidad { get; set; }
        public double MaderaCantidad { get; set; }
        public double CostalesCantidad { get; set; }
        public double PesoMinimo { get; set; }
    }

    public class SobrepesoAlert
    {
        public int SobrepesoAlerta(double sobrePeso, string proceso)
        {
            switch (proceso)
            {
                //Reglas para PPVC
                case "PPVC":
                    if (sobrePeso < 0) return 2;
                    else if (sobrePeso > 3.5) return 3;
                    else return 1;

                //Reglas para INYECCION
                case "INY":
                    if (sobrePeso == 0) return 1;
                    else if (sobrePeso > 0) return 3;
                    else return 2;

                //Reglas para CORRUGADO (falta que las establezcan, de momento usamos las mismas que en ppvc)
                case "Corrugado":
                    if (sobrePeso < 0) return 2;
                    else if (sobrePeso > 3.5) return 3;
                    else return 1;

                default:
                    return 1;
            }
        }
    }

    public class DataAddTicket   //recibe datos para crear un registro de escaneo en DB
    {
        public string? code { get; set; }
        public int linea { get; set; }
        public int planta { get; set; }
        public int turno { get; set; }
        public int calidad { get; set; }
        public double pesoNeto { get; set; }
        public double sobrePeso { get; set; }
        public double pesotubos { get; set; }
        public int numTubos { get; set; }
        public double pesoBascula { get; set; }
    }

    public class DataAddScrap   //recibe datos para crear un registro de escaneo en DB de SCRAP
    {
        public string? nameUser { get; set; }
        public string? code { get; set; }
        public double pesoBascula { get; set; }
        public int turno { get; set; }
        public string? proceso { get; set; }
        public string? coments { get; set; }
        public int linea { get; set; }
        public string? familia { get; set; }
        public int planta { get; set; }
        public int scrapCorteSierra { get; set; }

    }
}
