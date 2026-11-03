using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.TicketsRepo
{
    public interface IGenericRepoTickets<TEntityModel> where TEntityModel : class
    {
        IList<TEntityModel> GetAll(int planta);
        IList<TEntityModel> GetAll2(int planta, string proceso);
        Task<bool> InsertTickey(TEntityModel entity);
        IList<TEntityModel> GetTicketByLinie(int linie);
        IList<TEntityModel> GetTickeyById(int id);
        IList<TEntityModel> GetPrintTickeyById(int id);
        Task<bool> UpdateTicket(int idTicket, float peso, float pesobascula, float porcentaje, bool calidad, decimal pesotubos, string? tubos, int planta);
        IList<TEntityModel> FilterTickets(string? folio, int? turno, string? fechaInicio, string? fechaFin, int planta);
        IList<TEntityModel> FilterTickets2(string? folio, int? turno, string? fechaInicio, string? fechaFin, int planta, string proceso);
    }
}
