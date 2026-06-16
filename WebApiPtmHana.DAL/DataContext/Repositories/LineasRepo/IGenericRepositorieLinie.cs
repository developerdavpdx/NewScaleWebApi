using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.LineasRepo
{
    public interface IGenericRepositorieLinie<EntityModel> where EntityModel : class
    {
        IList<Lineas> GetLineas(string planta);
        Task<bool> AsignLineToSKU(string sku, int linie, string trabajador, int turno, decimal kgxhr);
        //Task<bool> UnassignLinie(int idLinea);
        Task<bool> UnassignLinieAndTicket(int idLinea, string planta);
        Task<bool> CreateNewLine(int linea, string proceso, string planta);
        public Lineas GetCode(string planta, string linea);
    }
}
