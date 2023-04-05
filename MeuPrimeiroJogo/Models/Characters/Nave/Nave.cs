using Asteroid.Enuns;
using Asteroid.Guis;
using Asteroid.Helpers;
using Asteroid.Models.Characters.Game;
using Asteroid.Models.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public bool IsBoss { get; set; }
        public bool SpecialShoot { get; set; }
        public Bullet Bullet { get; set; }
        public List<PowerUp> Powers { get; set; }
        private EnumMovement LastMoviment = (EnumMovement)(new Random().Next(1,8));
        public string GetTextureHit()
        {
            if (!IsHited)
            {
                return $"{this.TextureName}Hit";
            }
            else
            {
                return this.TextureName;
            }
        }
        public void Initialize(KeyboardState keyboardState, GraphicsDeviceManager graphics, GameScreen gameScreen, Texture2D texture, GameTime gameTime)
        {
            PlayerUsePowerUp(keyboardState, gameScreen, gameTime.ElapsedGameTime);
            PlayerMovement(keyboardState, graphics);
            CheckUnhit(texture, gameTime.ElapsedGameTime);
        }
        public void PlayerUsePowerUp(KeyboardState keyboardState, GameScreen gameScreen, TimeSpan ElapsedGameTime)
        {
            if (keyboardState.IsKeyDown(Keys.X))
            {
                if (Powers.Count > 0)
                {
                    if (!Powers.Any(x => x.Using))
                    {
                        Powers[0].UsePowerUp(this, gameScreen);
                        Powers[0].TimeElapsedPower += (int)ElapsedGameTime.TotalMilliseconds;                       
                    }                    
                }
            }

            if (Powers.Count > 0 && Powers.Any(x => x.Using))
            {
                var power = Powers.FirstOrDefault(x => x.Using);
                var indexPower = Powers.IndexOf(power);

                if (power.CheckTimeDisabled())
                {
                    Powers[indexPower].DisabledPower(this, gameScreen);
                    Powers.Remove(power);
                }
                else
                {
                    Powers[indexPower].TimeElapsedPower += (int)ElapsedGameTime.TotalMilliseconds;
                }
            }
        }
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
            var randomNumber = Random.Shared.Next(1, 8);
            var randomMoviment = (EnumMovement)randomNumber;

            if (this.ElapsedTimeSinceLastMovement >= this.TimeBetweenMovement)
            {                
                if (randomMoviment != LastMoviment)
                {
                    this.Moviment(randomMoviment, this.Speed, graphics);
                    LastMoviment = randomMoviment;
                }

                this.ElapsedTimeSinceLastMovement = 0;
            }
            else
            {                
                this.ElapsedTimeSinceLastMovement += (int)ElapsedGameTime.TotalMilliseconds;
                this.Moviment(LastMoviment, this.Speed, graphics);
            }

            var width = graphics.PreferredBackBufferWidth - 100;
            var heigth = graphics.PreferredBackBufferHeight - 300;
            ScreenLimit(width, heigth);
        }
    }
}
