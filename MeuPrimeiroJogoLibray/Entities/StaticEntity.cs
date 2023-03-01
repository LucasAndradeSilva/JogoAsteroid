using MeuPrimeiroJogoLibray.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeuPrimeiroJogoLibrary.Entities
{
    public class StaticEntity : Entity
    {
        public Texture2D Texture { get; set; }
        public StaticEntity(StaticEntity source) : base(source)
        {
        }

        public StaticEntity(string name, string texturePath, Game game) : base(name, game)
        {
            Texture = game.Content.Load<Texture2D>(texturePath);
        }
    }
}
