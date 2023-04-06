using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Asteroid.Gui.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asteroid.Gui.Models.Screens;
using Asteroid.Gui.Models.Elements;
using Microsoft.Xna.Framework.Input.Touch;


namespace Asteroid.Gui.Guis
{
    public class StartScreen : Screen
    {
        Button BtnStart;
        Button BtnStartTwoPlayer;
        Background Background;
        Text TxtStartTitulo;
        Text TxtStartSubTitulo;

        public StartScreen(AsteroidGame game) : base(game)
        {
            game.Window.Title = "Inicio";
            var WidthBtn = game.IsMobile ? 400 : 200;
            var HeigthBtn = game.IsMobile ? 200 : 100;

            BtnStart = new Button()
            {
                X = game.graphics.GetCenterX() - 60,
                Y = game.graphics.GetCenterY() - 20,
                Width = WidthBtn,
                Heigth = HeigthBtn
            };
            BtnStartTwoPlayer = new Button()
            {
                X = game.graphics.GetCenterX() + 80,
                Y = game.graphics.GetCenterY() - 20,
                Width = 200,
                Heigth = 100
            };
            Background = new Background()
            {
                Width = game.graphics.PreferredBackBufferWidth,
                Heigth = game.graphics.PreferredBackBufferHeight
            };

            var StartTituloX = game.IsMobile ? game.graphics.GetCenterX() + 20 : game.graphics.GetCenterX() - 10;
            
            TxtStartTitulo = new Text()
            {
                X = StartTituloX,
                Y = game.graphics.GetCenterY() - 80,
                Content = "Asteroid Game"
            };

            var StartSubX = game.IsMobile ? game.graphics.GetCenterX() + 20 : game.graphics.GetCenterX();
            TxtStartSubTitulo = new Text()
            {
                X = StartSubX,
                Y = game.graphics.GetCenterY() - 40,
                Content = "Criado por Lucas Andrade"
            };
        }

        public override void LoadContent()
        {            
            BtnStart.Texture = game.Content.Load<Texture2D>("Images/btnStart");
            BtnStartTwoPlayer.Texture = game.Content.Load<Texture2D>("Images/btnStart");
            Background.Texture = game.Content.Load<Texture2D>("Images/fundo1");

            TxtStartTitulo.SpriteFont = game.Content.Load<SpriteFont>("fontes/titulo");
            TxtStartSubTitulo.SpriteFont = game.Content.Load<SpriteFont>("fontes/arial");

            game.IsMouseVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var touchState = TouchPanel.GetState().FirstOrDefault();            

            if (!game.IsMobile)
            {
                //Muda cursor
                BtnStart.Hover(mouseState,
                    () =>
                    {
                        Mouse.SetCursor(MouseCursor.Hand);
                    },
                    () =>
                    {
                        Mouse.SetCursor(MouseCursor.Arrow);
                    }
                );

                BtnStartTwoPlayer.Click(game, touchState, mouseState, () =>
                {
                    game.TwoPlayers = true;
                    game.currentScreen = new PreGameScreen(game);
                    game.currentScreen.LoadContent();
                });
            }

            //Clique no botão
            BtnStart.Click(game, touchState, mouseState, () =>
            {
                game.currentScreen = new PreGameScreen(game);
                game.currentScreen.LoadContent();
            });
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Desenha o fundo
            spriteBatch.DrawElement(Background);

            // Desenha o botão de iniciar
            spriteBatch.DrawElement(BtnStart);
            if (!game.IsMobile)
            {
                spriteBatch.DrawElement(BtnStartTwoPlayer);
            }

            //Desenha Titulo
            spriteBatch.DrawText(TxtStartTitulo);
            spriteBatch.DrawText(TxtStartSubTitulo);

            game.spriteBatch = spriteBatch;
        }
    }

}

