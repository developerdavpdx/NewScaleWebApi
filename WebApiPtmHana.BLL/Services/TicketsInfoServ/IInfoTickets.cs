using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.TicketsInfoServ
{
    public interface IInfoTickets
    {
        IList<TicketsInfo> GetTickets(int planta);
        IList<TicketsInfo> GetTicketByLinie(int linie);
        Task<bool> AddNewTicket(TicketsInfo ticket);
        IList<TicketsInfo> GetTicketById(int id);
        Task<bool> UpdateTicket(int idTicket, float peso, float pesoBascula, float porcentaje, bool calidad, decimal pesotubos, string? tubos, int planta);
        IList<TicketsInfo> GetPrintTickeyById(int id);
        IList<TicketsInfo> FilterTickest(int? turno, string? folio, string? fechaInicio, string? fechaFin, int planta);
    }
}
