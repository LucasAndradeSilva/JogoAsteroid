using Asteroid.Dao.Elements;
using Asteroid.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Windows
{
    public class StartScreen : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Button BtnStart;
        Background Background;
        Text TxtStartTitulo;
        Text TxtStartSubTitulo;

        public StartScreen()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {            
            BtnStart = new Button()
            {
                X = graphics.GetCenterX(),
                Y = graphics.GetCenterY() - 20,
                Width = 200,
                Heigth = 100
            };
            Background = new Background()
            {
                Width = graphics.PreferredBackBufferWidth,
                Heigth = graphics.PreferredBackBufferHeight
            };
            TxtStartTitulo = new Text()
            {
                X = graphics.GetCenterX() - 10,
                Y = graphics.GetCenterY() - 80,                
                Content = "Asteroid Game"                
            };
            TxtStartSubTitulo = new Text()
            {
                X = graphics.GetCenterX(),
                Y = graphics.GetCenterY() - 40,                
                Content = "Criado por Lucas Andrade"
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            BtnStart.Texture = Content.Load<Texture2D>("images/btnStart");
            Background.Texture = Content.Load<Texture2D>("images/fundo");

            TxtStartTitulo.SpriteFont = Content.Load<SpriteFont>("fontes/titulo");
            TxtStartSubTitulo.SpriteFont = Content.Load<SpriteFont>("fontes/arial");

            IsMouseVisible = true;
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            //Muda cursor
            BtnStart.Hover(mouseState, 
                () => {
                    Mouse.SetCursor(MouseCursor.Hand);
                },
                () => {
                    Mouse.SetCursor(MouseCursor.Arrow);
                }
            );

            //Clique no botão
            BtnStart.Click(mouseState, () =>
            {
                var GameScreen = new GameScreen();
                GameScreen.Run();
            });

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            // Desenha o fundo
            spriteBatch.DrawElement(Background);

            // Desenha o botão de iniciar
            spriteBatch.DrawElement(BtnStart);

            //Desenha Titulo
            spriteBatch.DrawText(TxtStartTitulo);
            spriteBatch.DrawText(TxtStartSubTitulo);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}

