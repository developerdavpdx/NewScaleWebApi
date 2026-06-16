using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.TicketsRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.TicketsInfoServ
{
    public class InfoTicketsService : IInfoTickets
    {
        private readonly IGenericRepoTickets<TicketsInfo> _repositorie;
        public InfoTicketsService(IGenericRepoTickets<TicketsInfo> repositorie)
        {
            _repositorie = repositorie;
        }
        public async Task<bool> AddNewTicket(TicketsInfo ticket)
        {
            return await _repositorie.InsertTickey(ticket);
        }

        public IList<TicketsInfo> FilterTickest(int? turno, string? folio, string? fechaInicio, string? fechaFin, int planta)
        {
            return _repositorie.FilterTickets(folio, turno, fechaInicio, fechaFin, planta);
        }

        public IList<TicketsInfo> GetPrintTickeyById(int id)
        {
            return _repositorie.GetPrintTickeyById(id);
        }

        public IList<TicketsInfo> GetTicketById(int id)
        {
            return _repositorie.GetTickeyById(id);
        }

        public IList<TicketsInfo> GetTicketByLinie(int linie)
        {
            return _repositorie.GetTicketByLinie(linie);
        }

        public IList<TicketsInfo> GetTickets(int planta)
        {
            return _repositorie.GetAll(planta);
        }

        public Task<bool> UpdateTicket(int idTicket, float peso,float pesoBascula, float porcentaje, bool calidad, decimal pesotubos, string? tubos, int planta)
        {
            return _repositorie.UpdateTicket(idTicket, peso,pesoBascula, porcentaje, calidad, pesotubos, tubos, planta);
        }
    }
}
