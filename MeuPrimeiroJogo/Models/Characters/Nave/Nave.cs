using Asteroid.Models.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Characters.Nave
{
    public class Nave : Character
    {
        public int ElapsedTimeSinceLastMovement { get; set; }
        public int TimeBetweenMovement { get; set; }
        public int ElapsedDirection { get; set; }
        public Bullet Bullet { get; set; }
        public void PlayerMovement(KeyboardState keyboardState, GraphicsDeviceManager graphics)
        {
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                this.X -= this.Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                this.X += this.Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {   
                this.Y -= this.Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                this.Y += this.Speed;
            }

            ScreenLimit(graphics);
        }          
        
        public void AutoMovement(GraphicsDeviceManager graphics, TimeSpan ElapsedGameTime)
        {
            if (this.ElapsedTimeSinceLastMovement >= this.TimeBetweenMovement)
            {
                var random = new Random();
                var randomNumber = random.Next(0, 400);
                if (randomNumber <= 200)
                {
                    this.Moviment(Enuns.EnumMovement.Right, this.Speed, graphics);
                }
                else if(randomNumber >= 200 && randomNumber > 800) { 
                    this.Moviment(Enuns.EnumMovement.Left, this.Speed, graphics);
                }
                //else if (randomNumber > 50 && randomNumber <= 75)
                //{
                //    this.Moviment(Enuns.EnumMovement.Up, this.Speed, graphics);
                //}
                //else
                //{
                //    this.Moviment(Enuns.EnumMovement.Down, this.Speed, graphics);
                //}
            }
            else
            {
                this.ElapsedTimeSinceLastMovement += (int)ElapsedGameTime.TotalMilliseconds;
            }
            ScreenLimit(graphics);
        }
    }
}
