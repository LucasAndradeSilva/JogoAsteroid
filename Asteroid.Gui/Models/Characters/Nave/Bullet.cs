using Asteroid.Gui.Enuns;
using Asteroid.Gui.Models.Characters.Game;
using Asteroid.Gui.Models.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Gui.Models.Characters.Nave
{
    public class Bullet : Character
    {
        public EnumMovement Movement { get; set; } = default;
        public int ElapsedTimeSinceLastShot { get; set; }
        public int TimeBetweenShots { get; set; }
        public int ElapsedTimeSinceLastShot2 { get; set; }
        public int TimeBetweenShots2 { get; set; }
        public List<Bullet> Bullets { get; set; }
        public void BulletShoot(KeyboardState keyboardState, TimeSpan ElapsedGameTime, Nave nave)
        {
            if (keyboardState.IsKeyDown(Keys.Enter) && this.ElapsedTimeSinceLastShot >= this.TimeBetweenShots)
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
        public void BulletShoot2(KeyboardState keyboardState, TimeSpan ElapsedGameTime, Nave nave)
        {
            if (keyboardState.IsKeyDown(Keys.Space) && this.ElapsedTimeSinceLastShot2 >= this.TimeBetweenShots2)
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
                this.ElapsedTimeSinceLastShot2 += (int)ElapsedGameTime.TotalMilliseconds;
            }
        }
        private void BulletShoot(int X, int Y, int Width, Nave nave)
        {
            var bullet = new Bullet()
            {
                X = X + Width / 2 - 4,
                Y = Y,
                Width = this.Width,
                Heigth = this.Heigth
            };

            if (nave.SpecialShoot && nave.Enemy)
                bullet.Movement = RandomMovimentExceptUp();
            else if (nave.SpecialShoot && !nave.Enemy)
                bullet.Movement = RandomMovimentExceptDown();

            this.Bullets.Add(bullet);

            this.ElapsedTimeSinceLastShot = 0;
            this.ElapsedTimeSinceLastShot2 = 0;
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
            var movement = EnumMovement.Up;
            var found = false;
            while (!found)
            {
                movement = (EnumMovement)Random.Shared.Next(1, 8);
                if (movement == EnumMovement.Down || movement == EnumMovement.LeftDown || movement == EnumMovement.RightDown || movement == EnumMovement.Right || movement == EnumMovement.Left)
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
        public EnumMovement RandomMovimentExceptUp()
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
                BulletShoot(nave.X, nave.Y, nave.Width, nave);            
        }        
        public void CheckShoot(Nave nave)
        {
            if (nave.IsBoss)            
                CreateShoots(Random.Shared.Next(1, 4), nave);            
            else            
                CreateShoots(1, nave);            
        }
    }
}
