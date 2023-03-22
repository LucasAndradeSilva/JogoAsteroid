using Asteroid.Models.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Characters
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
        public int Speed { get; set; } = 5;
        public int Size { get; set; } = 64;
        public Life Life { get; set; }
        public int Points { get; set; } = 10;
        public bool Enemy { get; set; } = false;

        public bool CheckCollision(Rectangle rectangleCollision)
        {
            return this.Rectangle.Intersects(rectangleCollision);
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
        public bool FisrtDraw { get; set; } = false;

        public void CreateLifes(int qtdLifes = 3)
        {
            for (int i = 0; i <= qtdLifes; i++)
            {
                var lifeClone = this.MemberwiseClone() as Life;
                lifeClone.X = this.X;
                lifeClone.Y = this.Y;
                lifeClone.Width = this.Width;
                lifeClone.Heigth = this.Heigth;
                lifeClone.Texture = this.Texture;
            
                Lifes.Add(lifeClone);
            }
        }
    }
}
