﻿using Asteroid.Models.Elements;
using Asteroid.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asteroid.Models.Screens;
using Asteroid.Guis;

namespace Asteroid.Guis
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

            BtnStart = new Button()
            {
                X = game.graphics.GetCenterX() - 60,
                Y = game.graphics.GetCenterY() - 20,
                Width = 200,
                Heigth = 100
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
            TxtStartTitulo = new Text()
            {
                X = game.graphics.GetCenterX() - 10,
                Y = game.graphics.GetCenterY() - 80,
                Content = "Asteroid Game"
            };
            TxtStartSubTitulo = new Text()
            {
                X = game.graphics.GetCenterX(),
                Y = game.graphics.GetCenterY() - 40,
                Content = "Criado por Lucas Andrade"
            };
        }

        public override void LoadContent()
        {
            BtnStart.Texture = game.Content.Load<Texture2D>("images/btnStart");
            BtnStartTwoPlayer.Texture = game.Content.Load<Texture2D>("images/btnStart");
            Background.Texture = game.Content.Load<Texture2D>("images/fundo1");

            TxtStartTitulo.SpriteFont = game.Content.Load<SpriteFont>("fontes/titulo");
            TxtStartSubTitulo.SpriteFont = game.Content.Load<SpriteFont>("fontes/arial");

            game.IsMouseVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

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

            //Clique no botão
            BtnStart.Click(mouseState, () =>
            {
                game.currentScreen = new PreGameScreen(game);
                game.currentScreen.LoadContent();
            });

            BtnStartTwoPlayer.Click(mouseState, () =>
            {
                game.TwoPlayers = true;
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
            spriteBatch.DrawElement(BtnStartTwoPlayer);            

            //Desenha Titulo
            spriteBatch.DrawText(TxtStartTitulo);
            spriteBatch.DrawText(TxtStartSubTitulo);

            game.spriteBatch = spriteBatch;
        }
    }

}
