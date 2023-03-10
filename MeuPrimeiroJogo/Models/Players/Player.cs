using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Players
{
    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public void UpdatePoints(int points)
        {
            this.Score += points;
        }
    }
}
