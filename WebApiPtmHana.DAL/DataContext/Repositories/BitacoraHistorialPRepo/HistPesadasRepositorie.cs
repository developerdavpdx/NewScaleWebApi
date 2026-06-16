using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.BitacoraHistorialPRepo
{
    public class HistPesadasRepositorie : IGenericRepositorieHistPesada<BitHistorialPesadas>
    {
        private readonly ApplicationDbContext _context;
        public HistPesadasRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }
        public IList<BitHistorialPesadas> GetLogHistoricalWeightById(int idHPesada)
        {
            IList<BitHistorialPesadas> historial = _context.BitHistorialPesadas
                .Where(x => x.IdHistorialPesada == idHPesada)
                .Select(x => new BitHistorialPesadas
                {
                    Id = x.Id,
                    IdHistorialPesada = x.IdHistorialPesada,
                    AnilloPTH = x.AnilloPTH,
                    AtadoPTH = x.AtadoPTH,
                    FechaModificacion = x.FechaModificacion,
                    FlejePTH = x.FlejePTH,
                    CodigoNombre = x.CodigoNombre,
                    Codigo = x.Codigo,
                    LineaPtH = x.LineaPtH,
                    Nombre = x.Nombre,
                    NumTubosPtH = x.NumTubosPtH,
                    PesoTotalPtH = x.PesoTotalPtH,
                    TaraPTH = x.TaraPTH
                })
                .ToList();

            return historial;
        }
    }
}
