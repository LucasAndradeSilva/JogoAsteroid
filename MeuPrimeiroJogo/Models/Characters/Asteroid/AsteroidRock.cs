﻿using Asteroid.Enuns;
using Asteroid.Models.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Characters.Asteroid
{
    public class AsteroidRock : Character
    {
        public int Count { get; set; }
        public bool Destroyed { get; set; } = false;
        public List<AsteroidRock> Asteroids { get; set; }

        public void CreateAsteroid(GraphicsDeviceManager graphics)
        {            
            if (Random.Shared.Next(100) < this.Count)
            {
                this.Size = Random.Shared.Next(44, 74);
                var asteroid = new AsteroidRock()
                {
                    X = Random.Shared.Next(graphics.PreferredBackBufferWidth - 64),
                    Y = -64,                    
                    Width = this.Size,
                    Heigth = this.Size,
					Points = Random.Shared.Next(8, 14),
					Texture = this.Texture                    
                };

                this.Asteroids.Add(asteroid);
            }             
        }        

        public void AsteroidMovement(GraphicsDeviceManager graphics, Action<dynamic> CallBackAction)
        {
            for (int i = this.Asteroids.Count - 1; i >= 0; i--)
            {
                var asteroid = this.Asteroids[i];

                this.Asteroids[i].Moviment(EnumMovement.Down, asteroid.Speed, graphics);

                var exitedScreen = this.Asteroids[i].CheckLeftScreen(graphics, EnumMovement.Down , () =>
                {
                    this.Asteroids.RemoveAt(i);
                    i--;
                });
                
                if (!exitedScreen)
                    CallBackAction(asteroid);
            }
        }

       
    }
}
