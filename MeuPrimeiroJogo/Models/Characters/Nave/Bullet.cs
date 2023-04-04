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
        public EnumMovement Movement { get; set; } = default;
        public int ElapsedTimeSinceLastShot { get; set; }
        public int TimeBetweenShots { get; set; }
        public List<Bullet> Bullets { get; set; }
        public void BulletShoot(KeyboardState keyboardState, TimeSpan ElapsedGameTime, Nave nave)
        {
            if (keyboardState.IsKeyDown(Keys.Space) && this.ElapsedTimeSinceLastShot >= this.TimeBetweenShots)
            {
                if (nave.SpecialShoot)
                {
                    CreateShoots(Random.Shared.Next(1, 4), nave);
                }
                else
                {
                    CreateShoots(1, nave);
                }                
            }
            else
            {
                this.ElapsedTimeSinceLastShot += (int)ElapsedGameTime.TotalMilliseconds;
            }
        }
        private void BulletShoot(int X, int Y, int Width, bool SpecialShoot = false)
        {
            var bullet = new Bullet()
            {
                X = X + Width / 2 - 4,
                Y = Y,
                Width = this.Width,
                Heigth = this.Heigth                
            };

            if (SpecialShoot)            
                bullet.Movement = RandomMovimentExceptDown();            

            this.Bullets.Add(bullet);

            this.ElapsedTimeSinceLastShot = 0;
        }
        public void AutoBulletShoot(TimeSpan ElapsedGameTime, Nave nave)
        {
            if (this.ElapsedTimeSinceLastShot >= this.TimeBetweenShots)
            {
                CheckShoot(nave);
            }
            else
            {
                this.ElapsedTimeSinceLastShot += (int)ElapsedGameTime.TotalMilliseconds;
            }
        }
        public void BulletShootMovement(GraphicsDeviceManager graphics, EnumMovement MovementPlayer, Action<dynamic> CallBackAction)
        {
            for (int i = this.Bullets.Count - 1; i >= 0; i--)
            {                
                var bullet = this.Bullets[i];
                var movement = bullet.Movement == EnumMovement.Nothing ? MovementPlayer : bullet.Movement;

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
        public EnumMovement RandomMovimentExceptDown()
        {
            var movement = EnumMovement.Down;
            var found = false;
            while (!found)
            {
                movement = (EnumMovement)Random.Shared.Next(1, 8);
                if (movement == EnumMovement.Up || movement == EnumMovement.LeftUp || movement == EnumMovement.RightUp || movement == EnumMovement.Right || movement == EnumMovement.Left)
                {
                    found = false;
                }
                else
                {                 
                    break;
                }
            }

            return movement;
        }
        private void CreateShoots(int qtdShoots, Nave nave)
        {
            for (int i = 0; i < qtdShoots; i++)            
                BulletShoot(nave.X, nave.Y, nave.Width, nave.SpecialShoot);            
        }        
        public void CheckShoot(Nave nave)
        {
            if (nave.IsBoss)
            {
                CreateShoots(Random.Shared.Next(1, 4), nave);
            }
            else
            {                
                CreateShoots(1, nave);
            }
        }
    }
}
