using Asteroid.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Characters
{
    public abstract class Character : Element
    {
        public int Speed { get; set; } = 5;
        public int Size { get; set; } = 64;
    }
}
