using Asteroid.Enuns;
using Asteroid.Models.Characters.Game;
using Asteroid.Models.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Characters.Nave
{
    public class Bullet : Character
    {       
        public int ElapsedTimeSinceLastShot { get; set; }
        public int TimeBetweenShots { get; set; }
        public List<Bullet> Bullets { get; set; }
        public void BulletShoot(KeyboardState keyboardState, TimeSpan ElapsedGameTime, Nave nave)
        {
            if (keyboardState.IsKeyDown(Keys.Space) && this.ElapsedTimeSinceLastShot >= this.TimeBetweenShots)
            {
                BulletShoot(nave.X, nave.Y, nave.Width);
            }
            else
            {
                this.ElapsedTimeSinceLastShot += (int)ElapsedGameTime.TotalMilliseconds;
            }
        }
        private void BulletShoot(int X, int Y, int Width)
        {
            var bullet = new Bullet()
            {
                X = X + Width / 2 - 4,
                Y = Y,
                Width = this.Width,
                Heigth = this.Heigth
            };

            this.Bullets.Add(bullet);

            this.ElapsedTimeSinceLastShot = 0;
        }
        public void AutoBulletShoot(TimeSpan ElapsedGameTime, Nave nave)
        {
            if (this.ElapsedTimeSinceLastShot >= this.TimeBetweenShots)
            {
                BulletShoot(nave.X, nave.Y, nave.Width);
            }
            else
            {
                this.ElapsedTimeSinceLastShot += (int)ElapsedGameTime.TotalMilliseconds;
            }
        }

        public void BulletShootMovement(GraphicsDeviceManager graphics, EnumMovement movement, Action<dynamic> CallBackAction)
        {
            for (int i = this.Bullets.Count - 1; i >= 0; i--)
            {                
                var bullet = this.Bullets[i];

                this.Bullets[i].Moviment(movement, bullet.Speed, graphics);

                var exitedScreen = this.Bullets[i].CheckLeftScreen(graphics, movement, () =>
                {
                    this.Bullets.RemoveAt(i);
                    i--;
                });

                if (!exitedScreen)
                    CallBackAction(bullet);
            }
        }
    }
}
