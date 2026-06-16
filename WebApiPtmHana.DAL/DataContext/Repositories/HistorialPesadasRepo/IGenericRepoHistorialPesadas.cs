using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.HistorialPesadasRepo
{
    public interface IGenericRepoHistorialPesadas<TEntityModel> where TEntityModel : class
    {
        IList<TEntityModel> GetAll(int planta);
        Task<bool> AddComent(int idHistPesada, string comentario);
        Task<bool> DisableRegister(int idHistPesada, string comentario);
        IList<TEntityModel> GetHistoryWeightById(int idHistPesada);
        Task<bool> EditRecordById(int idHistPesada, int? LineaPtH, float? PesoTotalPtH, int? NumTubosPtH, float? AtadoPTH, float? TaraPTH, float? AnilloPTH, float? FlejePTH, string comentario, string nombreTrabjador, int idSKU);
        bool CopyRegister(int idHPesada, string comentario, string Codigo, int Id_Linea,
            float PesoTotal, int NumTubos, float AtadoPT, float TaraPT, float AnilloPT, float FlejePT);
        IList<TEntityModel> FilterInfoHistorical(int? turno, string? codigo, int? idEstadoSap, string? fechaInicio, string? fechaFin, int planta, int? linea);
        Task<bool> UpdateSapMessage(string id, string descripcion);
    }
}
