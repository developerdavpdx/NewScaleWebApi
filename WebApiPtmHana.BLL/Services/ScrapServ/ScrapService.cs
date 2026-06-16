using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.ScarpRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.ScrapServ
{
    public class ScrapService : IScrap
    {
        private readonly IGenericRepositorieScrap<ScrapMolinos> _repositorie;

        public ScrapService(IGenericRepositorieScrap<ScrapMolinos> repositorie)
        {
            _repositorie = repositorie;
        }

        public async Task<bool> AddNewScrap(ScrapMolinos scrap)
        {
            return await _repositorie.AddNewScrap(scrap);
        }

        public async Task<bool> AddRemolidoScrap(ScrapMolinos scrap)
        {
            return await _repositorie.AddRemolido(scrap);
        }

        public IList<ScrapMolinos> FilterScrap(string? fechaInicio, string? fechaFin, int? turno, string? estado, string planta)
        {
            return _repositorie.FilterScrap(fechaInicio, fechaFin, turno, estado, planta);
        }

        public IList<ScrapMolinos> GetAllScrapMolinos(string planta)
        {
            return _repositorie.GetAllScrapMolinos(planta);
        }

        public int GetNextIdScrap()
        {
            return _repositorie.GetNextIdScrap();
        }

        public IEnumerable<object> GetSumLineas(int turno, DateTime horaTurnoStart, DateTime horaTurnoEnd)
        {
            return _repositorie.GetSumLineas(turno, horaTurnoStart, horaTurnoEnd);
        }
    }
}
