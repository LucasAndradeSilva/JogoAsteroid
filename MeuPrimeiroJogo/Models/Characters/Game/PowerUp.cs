using Asteroid.Enuns;
using Asteroid.Models.Elements;
using Asteroid.Models.Characters;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asteroid.Gui;

namespace Asteroid.Models.Characters.Game
{
    public abstract class PowerUp : Character
    {        
        public bool Keep { get; set; }
        public bool Using { get; set; }
        public int TimeElapsedPower { get; set; }
        public int TimeDisabledPower { get; set; }
        public EnumPowerUpType PowerUpType { get; set; }
        public abstract void ActionPower(Nave.Nave nave, GameScreen gameScreen);
        public abstract void DisebledPower(Nave.Nave nave, GameScreen gameScreen);
        public static PowerUp GeneretPowerUp()
        {
            var typePower = EnumPowerUpType.Nada;
            var group = Random.Shared.Next(1, 2);
            var probability = Random.Shared.Next(0, 100);

            if (group == 1)
            {                
                if (probability <= 20)
                {
                    typePower = (EnumPowerUpType)Random.Shared.Next(1, 3);
                }                
            }
            else
            {             
                if (probability <= 7)
                {
                    typePower = (EnumPowerUpType)Random.Shared.Next(4, 6);
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
                            TextureName = "images/Untitled",
                            Width = 50,
                            Heigth = 50,
                        };              
                    case EnumPowerUpType.Fire:
                        return new FireUp()
                        {
                            TextureName = "images/Untitled",
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
                            TextureName = "images/Untitled",    
                            Width = 50,
                            Heigth = 50,
                        };              
                    default:
                        break;
                }
            }

            return default;
        }
        public void UsePowerUp(Nave.Nave nave, GameScreen gameScreen)
        {            
            ActionPower(nave, gameScreen);            
        }
    }

    public class ShieldUp : PowerUp
    {
        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            
        }

        public override void DisebledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            throw new NotImplementedException();
        }
    }

    public class VelocityUp : PowerUp
    {
        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            nave.Speed += 10;
            Using = true;
        }

        public override void DisebledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            nave.Speed -= 10;
            Using = false;
        }
    }

    public class TripleShotUp : PowerUp
    {
        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            throw new NotImplementedException();
        }
        public override void DisebledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            throw new NotImplementedException();
        }
    }

    public class FireUp : PowerUp
    {
        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            throw new NotImplementedException();
        }
        public override void DisebledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            throw new NotImplementedException();
        }
    }

    public class LifeUp : PowerUp
    {
        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            Using = true;
            nave.Life.CreateLifes(1);
        }
        public override void DisebledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            Using = false; 
        }
    }

    public class NuclearUp : PowerUp
    {
        public override void ActionPower(Nave.Nave nave, GameScreen gameScreen)
        {
            throw new NotImplementedException();
        }
        public override void DisebledPower(Nave.Nave nave, GameScreen gameScreen)
        {
            throw new NotImplementedException();
        }
    }
}
