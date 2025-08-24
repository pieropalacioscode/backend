using DBModel.DB;
using IService;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MLPredictionService : IMLPredictionService
    {
        private readonly MLContext _mlContext;
        public MLPredictionService()
        {
            _mlContext = new MLContext(seed: 0); // Semilla fija para reproducibilidad
        }

        public byte[] TrainForecastingModel(List<VentaHistoricaDto> ventas, List<DimFecha> fechas, int horizon = 7)
        {
            if (ventas == null || ventas.Count < 10)
                throw new ArgumentException("Se necesitan al menos 10 registros para entrenar el modelo.");

            // Preparar datos - ORDENAR POR FECHA MUY IMPORTANTE
            var data = ventas.OrderBy(v => v.Fecha).Select(v =>
            {
                var fechaInfo = fechas.FirstOrDefault(f => f.Fecha.Date == v.Fecha.Date);
                return new VentasData
                {
                    Fecha = v.Fecha,
                    Cantidad = (float)v.Cantidad,
                    // Convertir explícitamente a float para consistencia
                    EsFeriado = fechaInfo?.EsFeriado.GetValueOrDefault() == true ? 1.0f : 0.0f,
                    EsPrevioFeriado = fechaInfo?.EsPrevioFeriado.GetValueOrDefault() == true ? 1.0f : 0.0f,
                    EsDespuesFeriado = fechaInfo?.EsDespuesFeriado.GetValueOrDefault() == true ? 1.0f : 0.0f,
                    EsFinDeSemana = fechaInfo?.EsFinDeSemana.GetValueOrDefault() == true ? 1.0f : 0.0f,
                    Mes = (float)v.Fecha.Month,
                    Trimestre = (float)(fechaInfo?.Trimestre ?? ((v.Fecha.Month - 1) / 3 + 1)),
                    Estacion = fechaInfo?.Estacion ?? "Normal"
                };
            }).ToList();

            // Crear DataView
            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            // Pipeline mejorado
            var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "EstacionEncoded",
                    inputColumnName: nameof(VentasData.Estacion))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    nameof(VentasData.EsFeriado),
                    nameof(VentasData.EsPrevioFeriado),
                    nameof(VentasData.EsDespuesFeriado),
                    nameof(VentasData.EsFinDeSemana),
                    nameof(VentasData.Mes),
                    nameof(VentasData.Trimestre),
                    "EstacionEncoded"))
                .Append(_mlContext.Transforms.CopyColumns("Label", nameof(VentasData.Cantidad)))
                // Cambiar SDCA por FastTree que suele dar mejores resultados
                .Append(_mlContext.Regression.Trainers.FastTree(
                    numberOfLeaves: 20,
                    minimumExampleCountPerLeaf: 1,
                    learningRate: 0.2));

            // Entrenar
            var model = pipeline.Fit(dataView);

            // Serializar a bytes
            using var ms = new MemoryStream();
            _mlContext.Model.Save(model, dataView.Schema, ms);
            return ms.ToArray();
        }

        public float PredictFuture(byte[] modelBytes, DimFecha fecha)
        {
            using var ms = new MemoryStream(modelBytes);
            var model = _mlContext.Model.Load(ms, out _);

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<VentasData, VentasPrediction>(model);

            // Input coherente con entrenamiento - CONVERTIR TODO A FLOAT
            var input = new VentasData
            {
                Fecha = fecha?.Fecha ?? DateTime.Now, // Agregar fecha aunque no se use
                EsFeriado = fecha?.EsFeriado.GetValueOrDefault() == true ? 1.0f : 0.0f,
                EsPrevioFeriado = fecha?.EsPrevioFeriado.GetValueOrDefault() == true ? 1.0f : 0.0f,
                EsDespuesFeriado = fecha?.EsDespuesFeriado.GetValueOrDefault() == true ? 1.0f : 0.0f,
                EsFinDeSemana = fecha?.EsFinDeSemana.GetValueOrDefault() == true ? 1.0f : 0.0f,
                Mes = (float)(fecha?.Fecha.Month ?? DateTime.Now.Month),
                Trimestre = (float)(fecha?.Trimestre ?? (((fecha?.Fecha.Month ?? DateTime.Now.Month) - 1) / 3 + 1)),
                Estacion = fecha?.Estacion ?? "Normal"
            };

            var prediction = predictionEngine.Predict(input);

            // Evitar valores negativos si es necesario
            return Math.Max(0, prediction.Cantidad);
        }
    }

    // Clase para las predicciones - asegúrate de tenerla
    public class VentasPrediction
    {
        [ColumnName("Score")]
        public float Cantidad { get; set; }
    }
}