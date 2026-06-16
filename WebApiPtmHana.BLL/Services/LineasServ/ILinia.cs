using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.LineasServ
{
    public interface ILinia
    {
        Task<bool> AsignSkuToLinie(string sku, int linie, string trabajador, int turno, decimal kgxhr);
        IList<Lineas> CountLineas(string planta);
        //Task<bool> UnassignedLinie(int idLinea);
        Task<bool> UnassignLinieAndTicket(int idLinea, string planta);
        Task<bool> CreateNewLinie(int linea, string proceso, string planta);
        public Lineas GetCode(string planta, string linea);
    }
}
