using MeuPrimeiroJogoLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeuPrimeiroJogoLibray.Entities
{
    public class Entity
    {        
        public string Name { get; set; }
        public Game Game { get; set; }
        public Transform Transform { get; set; }
        public bool IsEnable { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public Entity(string name, Game game)
        {
            Name = name;
            Game = game;
        }

        public Entity(Entity source)
        {
            this.Name = source.Name;
            this.IsVisible = source.IsVisible;
            this.IsVisible = source.IsVisible;
            this.Game = source.Game;
            this.Transform = new Transform(source.Transform);
        }
        public void Update(GameTime gameTime)
        {
            if (this.IsEnable)
            {
                Transform.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch )
        {
            if (IsVisible)
            {

            }
        }
       
    }
}
