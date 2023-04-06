using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Gui.Models.Players
{
    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public string PathScores { 
            get {
                return Path.Combine(Environment.CurrentDirectory, "Scores");
            }
        }
        public string FileNameScores
        {
            get
            {
                return "Score.json";
            }
        }

        public string CompleteFileNameScores
        {
            get
            {
                return Path.Combine(PathScores, FileNameScores);
            }
        }

        public void UpdatePoints(int points)
        {
            this.Score += points;
        }
    }
}
