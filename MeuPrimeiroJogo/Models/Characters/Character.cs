using Asteroid.Models.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
        public int Life { get; set; } = 3;
        public int Points { get; set; } = 10;

        public bool CheckCollision(Rectangle rectangleCollision)
        {
            return this.Rectangle.Intersects(rectangleCollision);
        }
    }
}
