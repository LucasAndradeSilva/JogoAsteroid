using Asteroid.Gui.Models.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace Asteroid.Gui.Models.Characters
{
    public abstract class Character : Element
    {
        public Guid Id
        {
            get
            {
                return Guid.NewGuid();
            }
        }
        public string TextureName { get; set; }
        public int Speed { get; set; } = 5;
        public int Size { get; set; } = 64;
        public Life Life { get; set; }
        public int Points { get; set; } = 10;
        public bool Enemy { get; set; } = false;
        public bool Destroyed { get; set; } = false;        
        public bool IsHited { get; set; } = false;
        public bool Immune { get; set; }

        public int TimeBetweenHit { get; set; } = 1200;
        public int TimeElapsedHit { get; set; } = 0;


        public bool CheckCollision(Rectangle rectangleCollision)
        {
            return this.Rectangle.Intersects(rectangleCollision);
        }

        public void Hit(Texture2D hit)
        {
            this.Texture = hit;
            this.IsHited = true;
        }         
        
        public void CheckUnhit(Texture2D textura, TimeSpan ElapsedGameTime)
        {
            if (IsHited)
            {
                if (TimeElapsedHit > TimeBetweenHit)
                {
                    this.Texture = textura;
                    this.IsHited = false;
                    this.TimeElapsedHit = 0;
                }
                else
                {
                    this.TimeElapsedHit += (int)ElapsedGameTime.TotalMilliseconds;
                }
            }            
        }
    }

    public class Life : Element
    {
        public Life()
        {
            
        }
        public Guid Id
        {
            get
            {
                return Guid.NewGuid();
            }
        }
        public List<Life> Lifes { get; set; } = new List<Life> ();

        public void CreateLifes(int qtdLifes = 3)
        {
            for (int i = 0; i < qtdLifes; i++)
            {
                var lifeClone = new Life()
                {
                    X = this.X,
                    Y = this.Y,
                    Width = this.Width,
                    Heigth = this.Heigth,
                    Texture = this.Texture,
                    FisrtDraw = false,
                };          
            
                Lifes.Add(lifeClone);
            }
        }
    }
}
