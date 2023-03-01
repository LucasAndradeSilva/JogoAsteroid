using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidsGame
{
    public class GameScreen : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D startButtonTexture;
        Texture2D backgroundTexture;
        Rectangle startButtonRectangle;

        public GameScreen()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            startButtonRectangle = new Rectangle((graphics.PreferredBackBufferWidth - 200) / 2, (graphics.PreferredBackBufferHeight - 50) / 2, 200, 50);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            startButtonTexture = Content.Load<Texture2D>("images/btnStart");
            backgroundTexture = Content.Load<Texture2D>("images/fundo");            
            IsMouseVisible = true;
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            //Muda cursor
            if (startButtonRectangle.Contains(mouseState.Position)) {
                Mouse.SetCursor(MouseCursor.Hand);
            }
            else
            {
                Mouse.SetCursor(MouseCursor.Arrow);
            }

            //Clique no botão
            if (mouseState.LeftButton == ButtonState.Pressed && startButtonRectangle.Contains(mouseState.Position))
            {                
                // Iniciar o jogo
                Game2 game = new Game2();                
                game.Run();                
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            // Desenha o fundo
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            // Desenha o botão de iniciar
            spriteBatch.Draw(startButtonTexture, startButtonRectangle, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}

