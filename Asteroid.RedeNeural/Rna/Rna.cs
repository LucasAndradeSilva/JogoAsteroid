using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Asteroid.RedeNeural
{
    public class Rna
    {
        public MLContext mlContext;
        public TransformerChain<RegressionPredictionTransformer<Microsoft.ML.Trainers.LinearRegressionModelParameters>> model;
        private int Interations = 2000;

        public Rna()
        {
            mlContext = new MLContext();

            Trainer();
        }

        private void Trainer()
        {
            var trainingData = mlContext.Data.LoadFromEnumerable(GameData.GetDatas());

            var pipeline = mlContext
                .Transforms.Conversion.ConvertType("NaveX", outputKind: DataKind.Single)
                .Append(mlContext.Transforms.Conversion.ConvertType("NaveY", outputKind: DataKind.Single))
                .Append(mlContext.Transforms.Conversion.ConvertType("MeteoroX", outputKind: DataKind.Single))
                .Append(mlContext.Transforms.Conversion.ConvertType("MeteoroY", outputKind: DataKind.Single))
                .Append(mlContext.Transforms.Conversion.ConvertType("Direcao", outputKind: DataKind.Single))
                .Append(mlContext.Transforms.Concatenate("Features", "NaveX", "NaveY", "MeteoroX", "MeteoroY"))
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Direcao", maximumNumberOfIterations: Interations));

            model = pipeline.Fit(trainingData);
        }

        public void Prediction(GameData game, Action<dynamic> CallBackAction)
        {
            var prediction = mlContext.Model.CreatePredictionEngine<GameData, Prediction>(model).Predict(game);
            CallBackAction(prediction);
        }
    }
    public class GameData
    {
        public int NaveX { get; set; }
        public int NaveY { get; set; }
        public int MeteoroX { get; set; }
        public int MeteoroY { get; set; }
        public int Direcao { get; set; }
        private static int Interations = 2000;
        public static List<GameData> GetDatas()
        {
            var datas = new List<GameData>();
            for (int i = 0; i < Interations; i++)
            {
                var randNave = GetRandomPosition();
                var randMeteor = GetRandomPosition();

                var data = new GameData();

                data.MeteoroY = randMeteor.y;
                data.MeteoroX = randMeteor.x;
                data.NaveX = randNave.x;
                data.NaveY = randNave.y;

                if (data.MeteoroX >= (data.NaveX - 40) && data.MeteoroX <= (data.NaveX + 40))
                {
                    var direcao = Random.Shared.Next(1, 3);
                    data.Direcao = direcao;
                }
                else
                {
                    data.Direcao = 0;
                }

                datas.Add(data);
            }

            return datas;
        }
        public static (int x, int y) GetRandomPosition()
        {
            var randX = Random.Shared.Next(0, 600);
            var randY = Random.Shared.Next(0, 1000);

            return (randX, randY);
        }
    }

    public class Prediction
    {
        [ColumnName("Score")]
        public float Direcao { get; set; }
    }
}
