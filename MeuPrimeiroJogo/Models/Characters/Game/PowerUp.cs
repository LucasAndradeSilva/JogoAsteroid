using Asteroid.Enuns;
using Asteroid.Models.Elements;
using Asteroid.Models.Characters;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asteroid.Guis;

namespace Asteroid.Models.Characters.Game
{
    public abstract class PowerUp : Character
    {        
        public bool Keep { get; set; }
        public bool Using { get; set; }
        public int TimeElapsedPower { get; set; }
        public int TimeDisabledPower { get; set; } = 5000;
        public EnumPowerUpType PowerUpType { get; set; }
        public abstract void ActionPower(Nave.Nave nave, GameScreen gameScreen);
        public abstract void DisabledPower(Nave.Nave nave, GameScreen gameScreen);
        public abstract void ActionPower(Nave.Nave nave, GameScreenPlayers gameScreen);
        public abstract void DisabledPower(Nave.Nave nave, GameScreenPlayers gameScreen);
        public static PowerUp GeneretPowerUp()
        {
            var typePower = EnumPowerUpType.Nada;
            var group = Random.Shared.Next(1, 4);
            var probability = Random.Shared.Next(0, 100);

            if (group == 1)
            {
                if (probability <= 10)
                {
                    typePower = (EnumPowerUpType)Random.Shared.Next(4, 7);
                }
            }
            else
            {
                if (probability <= 15)
                {
                    typePower = (EnumPowerUpType)Random.Shared.Next(1, 3);
                }                
            }

            if (typePower != EnumPowerUpType.Nada)
            {
                switch (typePower)
                {
                    case EnumPowerUpType.Shield:
                        return new ShieldUp()
                        {
                            TextureName = "images/escudoUp",
                            Width = 50,
                            Heigth = 50,
                        };                    
                    case EnumPowerUpType.Velocity:
                        return new VelocityUp()
                        {
                            TextureName = "images/velodidadeUp",
                            Width = 50,
                            Heigth = 50,
                        };                                
                    case EnumPowerUpType.TripleShot:
                        return new TripleShotUp()
                        {
                            TextureName = "images/tripleUp",
                            Width = 50,
                            Heigth = 50,
                        };              
                    case EnumPowerUpType.Fire:
                        return new FireUp()
                        {
                            TextureName = "images/fireUp",
                            Width = 50,
                            Heigth = 50,
                        };
                    case EnumPowerUpType.Life:
                        return new LifeUp()
                        {
                            TextureName = "images/lifeUP",
                            Width = 50,
                            Heigth = 50,
                        };              
                    case EnumPowerUpType.Nuclear:
                        return new NuclearUp()
                        {
                            TextureName = "images/nuclearUp",    
                            Width = 50,
                            Heigth = 50,
                        };              
                    default:
                        break;
                }
            }

            return default;
        }
        public bool CheckTimeDisabled()
        {
            return this.TimeElapsedPower > this.TimeDisabledPower;
        }
        public void UsePowerUp(Nave.Nave nave, GameScreen gameScreen)
        {                        
            ActionPower(nave, gameScreen);         
        }
        public void UsePowerUp(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            ActionPower(nave, gameScreen);
        }
    }

    public class ShieldUp : PowerUp
    {
        public ShieldUp()
        {
            TimeDisabledPower = 15000;
        }

        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            nave.Immune = true;
            nave.Width = 94;
            nave.Heigth = 94;
            nave.TextureName = "images/naveShilded";
            Using = true;
        }

        public override void DisabledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            nave.Immune = false;
            nave.TextureName = "images/foguete";
            nave.Width = 64;
            nave.Heigth = 64;
            Using = false;
        }
         public override void ActionPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            nave.Immune = true;
            nave.Width = 94;
            nave.Heigth = 94;
            nave.TextureName = "images/naveShilded";
            Using = true;
        }

        public override void DisabledPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            nave.Immune = false;
            nave.TextureName = "images/foguete";
            nave.Width = 64;
            nave.Heigth = 64;
            Using = false;
        }
    }

    public class VelocityUp : PowerUp
    { 

        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            nave.Speed += 8;
            Using = true;
        }

        public override void DisabledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            nave.Speed -= 8;
            Using = false;
        }
        public override void ActionPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            nave.Speed += 8;
            Using = true;
        }

        public override void DisabledPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            nave.Speed -= 8;
            Using = false;
        }
    }

    public class TripleShotUp : PowerUp
    {
        public TripleShotUp()
        {
            TimeDisabledPower = 15000;
        }

        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            Using = true;
            nave.SpecialShoot = true;
        }
        public override void DisabledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            Using = false;
            nave.SpecialShoot = false;
        }
        public override void ActionPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            Using = true;
            nave.SpecialShoot = true;
        }
        public override void DisabledPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            Using = false;
            nave.SpecialShoot = false;
        }
    }

    public class FireUp : PowerUp
    {
        public FireUp()
        {
            TimeDisabledPower = 10000;   
        }
        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            nave.Bullet.Heigth = 80;
            nave.Bullet.Width = 60;
            nave.Bullet.TimeBetweenShots = 50;
            Using = true;
        }
        public override void DisabledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            nave.Bullet.Width = 8;
            nave.Bullet.TimeBetweenShots = 300;
            Using = false;
        }

        public override void ActionPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            nave.Bullet.Heigth = 80;
            nave.Bullet.Width = 60;
            nave.Bullet.TimeBetweenShots = 50;
            Using = true;
        }
        public override void DisabledPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            nave.Bullet.Width = 8;
            nave.Bullet.TimeBetweenShots = 300;
            Using = false;
        }
    }

    public class LifeUp : PowerUp
    {
        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            Using = true;
            nave.Life.CreateLifes(1);
        }
        public override void DisabledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            Using = false; 
        }

        public override void ActionPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            Using = true;
            nave.Life.CreateLifes(1);
        }
        public override void DisabledPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            Using = false;
        }
    }

    public class NuclearUp : PowerUp
    {
        public NuclearUp()
        {
            TimeDisabledPower = 500;
        }

        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            var sumPoints = gameScreen.AsteroidRock.Asteroids.Sum(x => x.Points);
            sumPoints += gameScreen.NavesEnemy.Sum(x => x.Points);
            sumPoints += gameScreen.Boss.Points;

            gameScreen.ClearNavesEnemy();
            gameScreen.ClearAsteroids();
            gameScreen.Boss = null;            
            Using = true;
        }
        public override void DisabledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            Using = false;
            gameScreen.ResetNavesEnemy();
            gameScreen.ResetAsteroids();
        }
        public override void ActionPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            var sumPoints = gameScreen.AsteroidRock.Asteroids.Sum(x => x.Points);
            sumPoints += gameScreen.NavesEnemy.Sum(x => x.Points);
            sumPoints += gameScreen.Boss.Points;

            gameScreen.ClearNavesEnemy();
            gameScreen.ClearAsteroids();
            gameScreen.Boss = null;
            Using = true;
        }
        public override void DisabledPower(Nave.Nave nave, GameScreenPlayers gameScreen)
        {
            Using = false;
            gameScreen.ResetNavesEnemy();
            gameScreen.ResetAsteroids();
        }
    }
}
