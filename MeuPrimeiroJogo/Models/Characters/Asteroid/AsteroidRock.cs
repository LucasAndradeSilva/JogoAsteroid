using Asteroid.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Characters.Asteroid
{
    public class AsteroidRock : Character
    {
        public int Count { get; set; }
        public List<AsteroidRock> Asteroids { get; set; }
    }
}
