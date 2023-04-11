using Accord.Math;
using Accord.Neuro;
using Asteroid.Gui.Models.Characters.Nave;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.RedeNeural.Learning
{
    public class NaveLearned
    {
        public static Nave LearnedMovements(Game.Game game , IA IA)
        {
            game.Nave.X = MathHelper.Clamp(game.Nave.X, 0, game.game.graphics.PreferredBackBufferWidth - game.Nave.Width);
            game.Nave.Y = MathHelper.Clamp(game.Nave.Y, 0, game.game.graphics.PreferredBackBufferHeight - game.Nave.Heigth);

            // Usar a rede neural para mover a nave
            double[] currentState = new double[IA.INPUT_SIZE];
            currentState[0] = game.Nave.X;
            currentState[1] = game.Nave.Y;
            currentState[2] = game.Nave.Speed;            
            
            var nearestRock = game?.AsteroidRock?.Asteroids?.OrderBy(x => (x.Vector - game.Nave.Vector).Length())?.FirstOrDefault();

            currentState[3] = nearestRock?.X ?? 0;
            currentState[4] = nearestRock?.Y ?? 0; 

            double[] action = IA.ActivationNetwork.Compute(currentState);

            var maxProbabilityIndex = action.IndexOf(action.Max());
            if (maxProbabilityIndex == 0)
            {
                game.Nave.X -= game.Nave.Speed;
            }
            else if (maxProbabilityIndex == 1)
            {
                game.Nave.X += game.Nave.Speed;
            }
            else if (maxProbabilityIndex == 2)
            {
                game.Nave.Y -= game.Nave.Speed;
            }
            else if (maxProbabilityIndex == 3)
            {
                game.Nave.Y += game.Nave.Speed;
            }
               
            game.Nave.X = MathHelper.Clamp(game.Nave.X, 0, game.game.graphics.PreferredBackBufferWidth - game.Nave.Width);
            game.Nave.Y = MathHelper.Clamp(game.Nave.Y, 0, game.game.graphics.PreferredBackBufferHeight - game.Nave.Heigth);

            return game.Nave;
        }
    }
}
