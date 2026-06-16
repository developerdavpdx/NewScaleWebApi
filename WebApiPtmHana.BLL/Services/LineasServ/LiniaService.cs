using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.LineasRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.LineasServ
{
    public class LiniaService : ILinia
    {
        private readonly IGenericRepositorieLinie<Lineas> _repositorie;
        public LiniaService(IGenericRepositorieLinie<Lineas> repositorie) 
        {
            _repositorie = repositorie;
        }

        public async Task<bool> AsignSkuToLinie(string sku, int linie, string trabajador, int turno, decimal kgxhr)
        {
            return await _repositorie.AsignLineToSKU(sku, linie, trabajador, turno, kgxhr);
        }

        public IList<Lineas> CountLineas(string planta)
        {
            return _repositorie.GetLineas(planta);
        }
        public Lineas GetCode(string planta, string linea)
        {
            return _repositorie.GetCode(planta, linea);
        }

        public async Task<bool> CreateNewLinie(int linea, string proceso, string planta)
        {
            return await _repositorie.CreateNewLine(linea, proceso, planta);
        }

        //public async Task<bool> UnassignedLinie(int idLinea)
        //{
        //    return await _repositorie.(idLinea);
        //}

        public async Task<bool> UnassignLinieAndTicket(int idLinea, string planta)
        {
            return await _repositorie.UnassignLinieAndTicket(idLinea, planta);
        }
    }
}
