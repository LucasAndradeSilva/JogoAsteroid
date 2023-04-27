using Accord.IO;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Asteroid.Gui.Models.Characters.Nave;
using Asteroid.RedeNeural.Learning;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.RedeNeural.Training
{
    public class NaveTraining
    {
        public static IA CreateBaseTrainingMoviments(Game.Game game, IA network, KeyboardState keyboardState)
        {
            // Verificar as teclas pressionadas e definir as variáveis de movimento da nave
            bool naveDeveMoverParaEsquerda = keyboardState.IsKeyDown(Keys.Left);
            bool naveDeveMoverParaDireita = keyboardState.IsKeyDown(Keys.Right);
            bool naveDeveMoverParaCima = keyboardState.IsKeyDown(Keys.Up);
            bool naveDeveMoverParaBaixo = keyboardState.IsKeyDown(Keys.Down);

            // Definir o estado atual do jogo como entrada da rede neural
            double[] currentState = new double[network.INPUT_SIZE];
            currentState[0] = game.Nave.X;
            currentState[1] = game.Nave.Y;
            currentState[2] = game.Nave.Speed;

            var nearestRock = game?.AsteroidRock?.Asteroids?.OrderBy(x => (x.Vector - game.Nave.Vector).Length())?.FirstOrDefault();

            currentState[3] = nearestRock?.X ?? 0;
            currentState[4] = nearestRock?.Y ?? 0;
            network.input[network.PositionIndex] = currentState;

            // Definir a ação correta como saída da rede neural
            double[] action = new double[network.OUTPUT_SIZE];
            if (naveDeveMoverParaEsquerda)
            {
                action[0] = 1;
            }
            else if (naveDeveMoverParaDireita)
            {
                action[1] = 1;
            }
            else if (naveDeveMoverParaCima)
            {
                action[2] = 1;
            }
            else if (naveDeveMoverParaBaixo)
            {
                action[3] = 1;
            }
            network.output[network.PositionIndex] = action;

            network.PositionIndex++;

            return network;
        }

        public static IA TrainingMoviments(IA network)
        {
            var teacher = new BackPropagationLearning(network.ActivationNetwork);
            teacher.LearningRate = network.learningRate;
            teacher.Momentum = network.momentum;            

            var error = double.MaxValue;
            var epoch = 0;

            while (epoch < network.epoch)
            {
                error = teacher.RunEpoch(network.input, network.output);
                epoch++;
            }

            network.SaveModel();

            network.Trained = true;

            return network;
        }        
    }
}
