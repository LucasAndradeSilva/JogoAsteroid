using Accord.Neuro;
using Asteroid.Gui.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.RedeNeural.Learning
{
    public class IA
    {
        // Definir as constantes da rede neural
        public int INPUT_SIZE = 5; // tamanho da entrada da rede neural (por exemplo, posição x, posição y, velocidade posição do inimigo mais próximo x, posição do inimigo mais próximo y)
        public int OUTPUT_SIZE = 4; // tamanho da saída da rede neural (movimentação para esquerda, direita, cima ou baixo)
        public int HIDDEN_LAYER_SIZE = 9; // tamanho da camada oculta da rede neural
        public int PositionIndex { get; set; }

        public double[][] input { get; set; }
        public double[][] output { get; set; }

        public double learningRate { get; set; } = 0.01;
        public double momentum { get; set; } = 0.9;
        public double epoch { get; set; } = 10000;

        public bool Trained { get; set; }

        public ActivationNetwork ActivationNetwork { get; set; }
        public IA CreateIA()
        {            
            // Inicializar a rede neural
            var neuralNetwork = new ActivationNetwork(
                new SigmoidFunction(), // função de ativação
                INPUT_SIZE, // tamanho da camada de entrada
                HIDDEN_LAYER_SIZE, // tamanho da camada oculta
                OUTPUT_SIZE); // tamanho da camada de saída

            var IA = new IA()
            {
                ActivationNetwork = neuralNetwork,
                input = new double[1000][],
                output = new double[1000][],
            };

            return IA;
        }
     
        public void SaveModel()
        {
            var modelName = $"modelo_{DateTime.Now.Ticks}.bin";
            var modelDirectory = Path.Combine(Environment.CurrentDirectory, "ModelosIA");
            var modelFolder = Path.Combine(modelDirectory, modelName);

            FileHelper.EnsureDirectoryExists(modelDirectory);

            using (FileStream fileStream = new FileStream(modelFolder, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, ActivationNetwork);
            }
        }

        public static ActivationNetwork LoadLastModel()
        {
            var modelDirectory = Path.Combine(Environment.CurrentDirectory, "ModelosIA");
            var lastModel = Directory.GetFiles(modelDirectory).FirstOrDefault();
            var network = Accord.IO.Serializer.Load<ActivationNetwork>(lastModel);
            return network;
        }
    }
}
