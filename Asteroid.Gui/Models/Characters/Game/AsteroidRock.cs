using Asteroid.Gui.Enuns;
using Asteroid.Gui.Models.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Gui.Models.Characters.Game
{
    public class AsteroidRock : Character
    {
        public int Count { get; set; }
        public List<AsteroidRock> Asteroids { get; set; }

        public void CreateAsteroid(GraphicsDeviceManager graphics)
        {
            if (Random.Shared.Next(100) < Count)
            {
                Size = Random.Shared.Next(44, 64);
                var asteroid = new AsteroidRock()
                {
                    X = Random.Shared.Next(graphics.PreferredBackBufferWidth - 64),
                    Y = -64,
                    Width = Size,
                    Heigth = Size,
                    Points = Random.Shared.Next(8, 14),
                    Texture = Texture
                };

                Asteroids.Add(asteroid);
            }
        }

        public void AsteroidMovement(GraphicsDeviceManager graphics, Action<dynamic> CallBackAction)
        {
            for (int i = Asteroids.Count - 1; i >= 0; i--)
            {
                var asteroid = Asteroids[i];

                Asteroids[i].Moviment(EnumMovement.Down, asteroid.Speed, graphics);

                var exitedScreen = Asteroids[i].CheckLeftScreen(graphics, EnumMovement.Down, () =>
                {
                    Asteroids.RemoveAt(i);
                    i--;
                });

                if (!exitedScreen)
                    CallBackAction(asteroid);
            }
        }


    }
}
