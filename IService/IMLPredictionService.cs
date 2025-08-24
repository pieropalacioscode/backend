using DBModel.DB;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IMLPredictionService
    {
        byte[] TrainForecastingModel(List<VentaHistoricaDto> ventas, List<DimFecha> fechas, int horizon = 7);
        float PredictFuture(byte[] modelBytes, DimFecha fecha);
    }
}
