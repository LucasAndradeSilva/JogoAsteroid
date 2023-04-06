using Asteroid.Gui.Helpers;
using Asteroid.Gui.Models.Characters;
using Asteroid.Gui.Models.Characters.Nave;
using Asteroid.Gui.Models.Elements;
using Asteroid.Gui.Models.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Gui.Guis
{
    public class PreGameScreen : Screen
    {
        #region Elements        
        Background Background;
        Text TxtCount;
        Text TxtScore;
        #endregion

        Nave Nave;
        Nave Nave2;

        int TimeBetweenText = 1000;
        int TimeBetweenTime = 0;

        public PreGameScreen(AsteroidGame game) : base(game)
        {
            game.Window.Title = "Prepare-se";

            Mouse.SetCursor(MouseCursor.Arrow);

            var SizeNave = game.IsMobile ? 100 : 64;

            Nave = new Nave()
            {
                Width = SizeNave,
                Heigth = SizeNave,
                Speed = 5,
                Size = SizeNave,
                X = game.graphics.GetCenterX() + 65,
                Y = game.graphics.GetCenterY() + 150,
            };
            Nave2 = new Nave()
            {
                Width = 64,
                Heigth = 64,
                Speed = 5,
                Size = 64,
                X = game.graphics.GetCenterX() + 65,
                Y = game.graphics.GetCenterY() + 150,
            };

            Background = new Background()
            {
                Width = game.graphics.PreferredBackBufferWidth,
                Heigth = game.graphics.PreferredBackBufferHeight
            };
            TxtCount = new Text
            {
                X = game.graphics.GetCenterX() + 60,
                Y = game.graphics.GetCenterY() - 30,
                Content = "5"
            };
            TxtScore = new Text
            {
                X = 10,
                Y = 10,
                Content = "Pontuacao 0"
            };
        }

        public override void LoadContent()
        {
            Nave.Texture = game.Content.Load<Texture2D>("Images/foguete");
            Nave2.Texture = game.Content.Load<Texture2D>("Images/foguete2");
            Background.Texture = game.Content.Load<Texture2D>("Images/fundo1");
            TxtCount.SpriteFont = game.Content.Load<SpriteFont>("fontes/super");
            TxtScore.SpriteFont = game.Content.Load<SpriteFont>("fontes/titulo");
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var touchState = TouchPanel.GetState().FirstOrDefault();

            if (TimeBetweenTime >= TimeBetweenText)
            {
                CountDownText();
                CheckEndCountDown();
            }

            Nave.PlayerMovement(touchState, keyboardState, game);
            if (game.TwoPlayers)
            {
                Nave2.PlayerMovement2(keyboardState, game.graphics);
            }

            TimeBetweenTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        private void CountDownText()
        {
            var CurrentNumber = int.Parse(TxtCount.Content) - 1;
            TxtCount.Content = CurrentNumber.ToString();
            TimeBetweenTime = 0;
        }

        private void CheckEndCountDown()
        {
            var CurrentNumber = int.Parse(TxtCount.Content) - 1;
            if (CurrentNumber < 0)
            {
                if (game.TwoPlayers)
                {
                    game.currentScreen = new GameScreenPlayers(game);
                    game.currentScreen.LoadContent();
                }
                else
                {
                    game.currentScreen = new GameScreen(game);
                    game.currentScreen.LoadContent();
                }                
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Desenha o fundo
            spriteBatch.DrawElement(Background);

            //Desenha nave
            spriteBatch.DrawElement(Nave);
            if (game.TwoPlayers)            
            {
                spriteBatch.DrawElement(Nave2);
            }

                //Desenha o texto
                spriteBatch.DrawText(TxtCount);
            spriteBatch.DrawText(TxtScore);

            game.spriteBatch = spriteBatch;
        }
    }
}
