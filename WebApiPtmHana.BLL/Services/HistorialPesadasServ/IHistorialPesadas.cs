using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.HistorialPesadasServ
{
    public interface IHistorialPesadas
    {
        IList<HistorialPesadas> GetAll(int planta);
        Task<bool> AddComent(int idHistPesada, string comentario);
        Task<bool> DisabledRegister(int idHistPesada, string comentario);
        IList<HistorialPesadas> GetHistoryWeigthById(int idHistPesada);
        Task<bool> EditRecordById(int idHistPesada, int? LineaPtH, float? PesoTotalPtH, int? NumTubosPtH, float? AtadoPTH, float? TaraPTH, float? AnilloPTH, float? FlejePTH, string comentario, string nombreTrabjador, int idSKU);
        bool CopyRecord(int idHPesada, string comentario, string Codigo, int Id_Linea, float PesoTotal, int NumTubos, float AtadoPT, float TaraPT, float AnilloPT, float FlejePT);
        IList<HistorialPesadas> FilterInfoHistorical(int? turno, string? codigo, int? idEstadoSap, string? fechaInicio, string? fechaFin, int planta, int? linea);
        Task<bool> UpdateSapMessage(string id, string descripcion);
    }
}
