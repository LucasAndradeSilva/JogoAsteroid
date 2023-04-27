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
        public int INPUT_SIZE { get; set; } = 5; // tamanho da entrada da rede neural (por exemplo, posição x, posição y, velocidade posição do inimigo mais próximo x, posição do inimigo mais próximo y)
        public int OUTPUT_SIZE { get; set; } = 4; // tamanho da saída da rede neural (movimentação para esquerda, direita, cima ou baixo)
        public int HIDDEN_LAYER_SIZE { get; set; } = 9; // tamanho da camada oculta da rede neural
        public int PositionIndex { get; set; }

        public double[][] input { get; set; }
        public double[][] output { get; set; }

        public double learningRate { get; set; } = 1;
        public double momentum { get; set; } = 0.9;
        public double epoch { get; set; } = 10000;
        public int qtdDate { get; set; } = 2000;
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
                input = new double[qtdDate][],
                output = new double[qtdDate][],
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

        public void SaveBaseTrained()
        {
            var modelNameInput = $"input_{DateTime.Now.Ticks}.json";
            var modelNameOutput = $"output_{DateTime.Now.Ticks}.json";
            var modelDirectory = Path.Combine(Environment.CurrentDirectory, "BaseTrainingIA");
            var modelDirectoryInput = Path.Combine(Environment.CurrentDirectory, "BaseTrainingIA", "Input");
            var modelDirectoryOutput = Path.Combine(Environment.CurrentDirectory, "BaseTrainingIA", "Output");

            FileHelper.EnsureDirectoryExists(modelDirectory);
            FileHelper.EnsureDirectoryExists(modelDirectoryInput);
            FileHelper.EnsureDirectoryExists(modelDirectoryOutput);

            var jsonInput = this.input.ToJson();
            FileHelper.WriterFile(modelDirectoryInput, modelNameInput, jsonInput);

            var jsonOutput = this.output.ToJson();
            FileHelper.WriterFile(modelDirectoryOutput, modelNameOutput, jsonOutput);
        }
        public static ActivationNetwork LoadLastModel()
        {
            var modelDirectory = Path.Combine(Environment.CurrentDirectory, "ModelosIA");
            var lastModel = Directory.GetFiles(modelDirectory).FirstOrDefault();
            var network = Accord.IO.Serializer.Load<ActivationNetwork>(lastModel);
            return network;
        }

        public bool HasBaseTrained()
        {
            var modelDirectory = Path.Combine(Environment.CurrentDirectory, "BaseTrainingIA", "Input");

            FileHelper.EnsureDirectoryExists(modelDirectory);

            var files = Directory.GetFiles(modelDirectory);
            var hasBaseTrained = (files?.Count() ?? 0) > 0;
            Trained = hasBaseTrained;
           
            return hasBaseTrained;
        }       

        public IA LoadDataTrained()
        {
            var modelDirectoryInput = Path.Combine(Environment.CurrentDirectory, "BaseTrainingIA", "Input");
            var modelDirectoryOutput = Path.Combine(Environment.CurrentDirectory, "BaseTrainingIA", "Output");

            FileHelper.EnsureDirectoryExists(modelDirectoryInput);
            FileHelper.EnsureDirectoryExists(modelDirectoryOutput);

            var filesInput = Directory.GetFiles(modelDirectoryInput);
            var filesOutput = Directory.GetFiles(modelDirectoryOutput);
            var inputFile = FileHelper.ReadFile(filesInput.FirstOrDefault());
            var outputFile = FileHelper.ReadFile(filesOutput.FirstOrDefault());
            var inputObg = inputFile.ToObject<double[][]>();
            var outputObg = outputFile.ToObject<double[][]>();

            this.input = inputObg;
            this.output = outputObg;

            return this;
        }
    }
}
