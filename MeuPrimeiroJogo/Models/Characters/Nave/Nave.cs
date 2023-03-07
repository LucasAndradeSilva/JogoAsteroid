using Asteroid.Models.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Characters.Nave
{
    public class Nave : Character
    {
        public Bullet Bullet { get; set; }
        public List<Bullet> Bullets { get; set; }
        public int ElapsedTimeSinceLastShot { get; set; }
        public int TimeBetweenShots { get; set; }
    }
}
