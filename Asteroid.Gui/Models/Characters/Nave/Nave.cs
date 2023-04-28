using Asteroid.Gui.Enuns;
using Asteroid.Gui.Guis;
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
using Microsoft.Xna.Framework.Input.Touch;
using Asteroid.Gui.Helpers;
using Asteroid.Gui.Models.Characters.Game;

namespace Asteroid.Gui.Models.Characters.Nave
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
        private EnumMovement LastMoviment = (EnumMovement)new Random().Next(1, 8);
        public string GetTextureHit()
        {
            if (!IsHited)
            {
                return $"{TextureName}Hit";
            }
            else
            {
                return TextureName;
            }
        }
        public void Initialize(TouchLocation touchLocation, KeyboardState keyboardState, AsteroidGame game, GameScreen gameScreen, Texture2D texture, GameTime gameTime)
        {
            PlayerUsePowerUp(keyboardState, gameScreen, gameTime.ElapsedGameTime);
            PlayerMovement(touchLocation, keyboardState, game);
            CheckUnhit(texture, gameTime.ElapsedGameTime);
        }
        public void Initialize(TouchLocation touchLocation, KeyboardState keyboardState, AsteroidGame game, GameScreenPlayers gameScreen, Texture2D texture, GameTime gameTime)
        {
            PlayerUsePowerUp(keyboardState, gameScreen, gameTime.ElapsedGameTime);
            PlayerMovement(touchLocation, keyboardState, game);
            CheckUnhit(texture, gameTime.ElapsedGameTime);
        }
        public void Initialize2(KeyboardState keyboardState, GraphicsDeviceManager graphics, GameScreenPlayers gameScreen, Texture2D texture, GameTime gameTime)
        {
            PlayerUsePowerUp2(keyboardState, gameScreen, gameTime.ElapsedGameTime);
            PlayerMovement2(keyboardState, graphics);
            CheckUnhit(texture, gameTime.ElapsedGameTime);
        }
        public void PlayerUsePowerUp(KeyboardState keyboardState, GameScreen gameScreen, TimeSpan ElapsedGameTime)
        {
            if (keyboardState.IsKeyDown(Keys.P))
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
        public void PlayerUsePowerUp(KeyboardState keyboardState, GameScreenPlayers gameScreen, TimeSpan ElapsedGameTime)
        {
            if (keyboardState.IsKeyDown(Keys.P))
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
        public void PlayerUsePowerUp2(KeyboardState keyboardState, GameScreenPlayers gameScreen, TimeSpan ElapsedGameTime)
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
        public void PlayerMovement(TouchLocation touchLocation, KeyboardState keyboardState, AsteroidGame game)
        {
            if (!game.IsMobile)
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    X -= Speed;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    X += Speed;
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    Y -= Speed;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    Y += Speed;
                }
            }
            else
            {
                var touchX = (int)touchLocation.Position.X;
                var touchY = (int)touchLocation.Position.Y - 200;

                X = touchX <= 0 ? X : touchX;
                Y = touchY <= 0 ? Y : touchY;
            }

            ScreenLimit(game.graphics);
        }
        public void PlayerMovement(TouchLocation touchLocation, KeyboardState keyboardState)
        {

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                X -= Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                X += Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                Y -= Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                Y += Speed;
            }

        }
        public void PlayerMovement2(KeyboardState keyboardState, GraphicsDeviceManager graphics)
        {
            if (keyboardState.IsKeyDown(Keys.A))
            {
                X -= Speed;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                X += Speed;
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                Y -= Speed;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Y += Speed;
            }

            ScreenLimit(graphics);
        }
        public void AutoMovement(GraphicsDeviceManager graphics, TimeSpan ElapsedGameTime)
        {
            var randomNumber = Random.Shared.Next(1, 8);
            var randomMoviment = (EnumMovement)randomNumber;

            if (ElapsedTimeSinceLastMovement >= TimeBetweenMovement)
            {
                if (randomMoviment != LastMoviment)
                {
                    Moviment(randomMoviment, Speed, graphics);
                    LastMoviment = randomMoviment;
                }

                ElapsedTimeSinceLastMovement = 0;
            }
            else
            {
                ElapsedTimeSinceLastMovement += (int)ElapsedGameTime.TotalMilliseconds;
                Moviment(LastMoviment, Speed, graphics);
            }

            var width = graphics.PreferredBackBufferWidth - 100;
            var heigth = graphics.PreferredBackBufferHeight - 300;
            ScreenLimit(width, heigth);
        }
    }
}
