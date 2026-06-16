using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.HistorialPesadasRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.HistorialPesadasServ
{
    public class HistorialPesadasService : IHistorialPesadas
    {
        public readonly IGenericRepoHistorialPesadas<HistorialPesadas> _repositorie;

        public HistorialPesadasService(IGenericRepoHistorialPesadas<HistorialPesadas> repositorie)
        {
            _repositorie = repositorie;
        }

        public async Task<bool> AddComent(int idHistPesada, string comentario)
        {
            return await _repositorie.AddComent(idHistPesada, comentario);
        }

        public bool CopyRecord(int idHPesada, string comentario, string Codigo, int Id_Linea,
            float PesoTotal, int NumTubos, float AtadoPT, float TaraPT, float AnilloPT, float FlejePT)
        {
            return _repositorie.CopyRegister(idHPesada, comentario, Codigo, Id_Linea, PesoTotal, NumTubos, AtadoPT, TaraPT, AnilloPT, FlejePT);
        }

        public async Task<bool> DisabledRegister(int idHistPesada, string comentario)
        {
            return await _repositorie.DisableRegister(idHistPesada, comentario);
        }

        public async Task<bool> EditRecordById(int idHistPesada, int? LineaPtH, float? PesoTotalPtH, int? NumTubosPtH, float? AtadoPTH, float? TaraPTH, float? AnilloPTH, float? FlejePTH, string comentario, string nombreTrabjador, int idSKU)
        {
            return await _repositorie.EditRecordById(idHistPesada, LineaPtH, PesoTotalPtH, NumTubosPtH, AtadoPTH, TaraPTH, AnilloPTH, FlejePTH, comentario, nombreTrabjador, idSKU);
        }

        public IList<HistorialPesadas> FilterInfoHistorical(int? turno, string? codigo, int? idEstadoSap, string? fechaInicio, string? fechaFin, int planta, int? linea)
        {
            return _repositorie.FilterInfoHistorical(turno, codigo, idEstadoSap, fechaInicio, fechaFin, planta, linea);
        }


        public IList<HistorialPesadas> GetAll(int planta)
        {
            return _repositorie.GetAll(planta);
        }

        public IList<HistorialPesadas> GetHistoryWeigthById(int idHistPesada)
        {
            return _repositorie.GetHistoryWeightById(idHistPesada);
        }

        public async Task<bool> UpdateSapMessage(string id, string descripcion)
        {
            return await _repositorie.UpdateSapMessage(id, descripcion);
        }
    }
}
