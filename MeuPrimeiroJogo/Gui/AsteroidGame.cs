using Asteroid.Models.Players;
using Asteroid.Models.Screens;
using Asteroid.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Gui
{
    public class AsteroidGame : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Screen currentScreen;
        public Player player;        

        public AsteroidGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
       
        protected override void Initialize()
        {
            player = new Player()
            {
                Name = "Guest"
            };
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            currentScreen = new StartScreen(this);
            currentScreen.LoadContent();
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            currentScreen.Draw(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }        
    }
}
