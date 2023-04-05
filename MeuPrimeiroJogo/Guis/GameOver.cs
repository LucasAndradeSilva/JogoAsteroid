using Asteroid.Models.Elements;
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
using Asteroid.Models.Players;

namespace Asteroid.Guis
{
    public class GameOver : Screen
    {
        Button BtnRestart;
        Button BtnSave;
        Button BtnStartWrite;
        Background Background;        

        Text TxtStartTitulo;
        Text TxtScoreTitulo;
        Text TxtScoreSubTitulo;
        Text TxtStartSubTitulo;
        TextInputComponent TxtInputName;

        List<Player> PlayersScore;

        SpriteFont ArialFont;

        Texture2D TextureEmpty;

        private int TimeBetweenClick = 500;
        private int TimeElapsedClick = 0;

        bool NameCompleted {
            get {
                return game.player.Name != "Guest" && !string.IsNullOrEmpty(game.player.Name.RemoveSpace());
            }
            set
            {
                NameCompleted = value;
            }
        }

        public GameOver(AsteroidGame game) : base(game)
        {
            game.Window.Title = "Score";

            BtnRestart = new Button()
            {
                X = game.graphics.GetCenterX(),
                Y = game.graphics.GetCenterY() + 40,
                Width = 200,
                Heigth = 100
            };
            BtnSave = new Button()
            {
                X = game.graphics.GetCenterX(),
                Y = game.graphics.GetCenterY() + 40,
                Width = 200,
                Heigth = 100
            };
            BtnStartWrite = new Button()
            {
                X = game.graphics.GetCenterX(),
                Y = game.graphics.GetCenterY() + 40,
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
                X = game.graphics.GetCenterX() - 355,
                Y = game.graphics.GetCenterY() - 150,
                Content = "Game Over"
            };
            TxtScoreTitulo = new Text()
            {
                X = game.graphics.GetCenterX() - 150,
                Y = game.graphics.GetCenterY() - 200,
                Content = "Score"
            };
            TxtScoreSubTitulo = new Text()
            {
                X = game.graphics.GetCenterX() - 100,
                Y = game.graphics.GetCenterY() - 50,
                Content = "Clique no botao e informe seu nome para gravarmos o score."
            };
            TxtStartSubTitulo = new Text()
            {
                X = game.graphics.GetCenterX(),
                Y = game.graphics.GetCenterY() + 20,
                Content = $"{game.player.Name} sua pontuacao foi de {game.player.Score}"
            };
            TxtInputName = new TextInputComponent()
            {
                X = game.graphics.GetCenterX(),
                Y = game.graphics.GetCenterY() + 20,
                TextInput = "Digite seu nome: "
            };

            TextureEmpty = new Texture2D(game.GraphicsDevice, 1, 1);

            PlayersScore = null;
        }

        public override void LoadContent()
        {
            ArialFont = game.Content.Load<SpriteFont>("fontes/arial");

            BtnRestart.Texture = game.Content.Load<Texture2D>("images/btnStart");
            BtnSave.Texture = game.Content.Load<Texture2D>("images/btnStart");
            BtnStartWrite.Texture = game.Content.Load<Texture2D>("images/btnStart");

            Background.Texture = game.Content.Load<Texture2D>("images/fundo1");

            TxtStartTitulo.SpriteFont = game.Content.Load<SpriteFont>("fontes/super");
            TxtStartSubTitulo.SpriteFont = ArialFont;

            TxtInputName.SpriteFont = ArialFont;

            TxtScoreTitulo.SpriteFont = game.Content.Load<SpriteFont>("fontes/super");
            TxtScoreSubTitulo.SpriteFont = ArialFont;

            game.IsMouseVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            CheckActionGui(gameTime);
        }
   
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Desenha o fundo
            spriteBatch.DrawElement(Background);

            if (NameCompleted)
            {
                // Desenha o botão de iniciar
                spriteBatch.DrawElement(BtnRestart);

                //Desenha Titulo
                spriteBatch.DrawText(TxtStartTitulo);
                spriteBatch.DrawText(TxtStartSubTitulo);

                //Desenha Score
                var lastY = game.graphics.GetCenterY() + 130;
                if (PlayersScore is not null)
                {
                    for (int i = 0; i < PlayersScore.Count; i++)
                    {
                        var position = i + 1;
                        var positionString = $"{position} {PlayersScore[i].Name} - {PlayersScore[i].Score}";

                        var TxtPosition = new Text();
                        TxtPosition.Content = positionString;
                        TxtPosition.X = game.graphics.GetCenterX() + 20;
                        lastY += 30;                        
                        TxtPosition.Y = lastY;                        
                        TxtPosition.SpriteFont = ArialFont;

                        TextureEmpty.SetData(new[]
                        {
                            Color.Black
                        });
                        var TxtPositionSize = TxtPosition.SpriteFont.MeasureString(positionString);

                        var TxtPositionBackgroud = new Background()
                        {
                            X = TxtPosition.X - 5,
                            Y = TxtPosition.Y - 5,
                            Width = (int)TxtPositionSize.X + 10,
                            Heigth  = (int)TxtPositionSize.Y + 10,
                            Color = Color.Black,
                            Texture = TextureEmpty
                        };

                        spriteBatch.DrawBackground(TxtPositionBackgroud);
                        spriteBatch.DrawText(TxtPosition);
                    }
                }

            }
            else
            {
                spriteBatch.DrawText(TxtScoreTitulo); 
                spriteBatch.DrawText(TxtScoreSubTitulo);

                if (TxtInputName.IsActive)
                {
                    spriteBatch.DrawElement(BtnSave);
                }
                else
                {
                    spriteBatch.DrawElement(BtnStartWrite);
                }
                
                TxtInputName.Draw(spriteBatch, new Vector2(game.graphics.GetCenterX() - 100, game.graphics.GetCenterY() + 20));
            }

            game.spriteBatch = spriteBatch;
        }

        private void CheckActionGui(GameTime gameTime)
        {
            if (NameCompleted)
                ActionGameOver(gameTime);
            else
                ActionCompleteName(gameTime);
        }
      
        private void ActionCompleteName(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            TxtInputName.Update(gameTime);
            
            BtnSave.Hover(mouseState,
                () =>
                {
                    Mouse.SetCursor(MouseCursor.Hand);
                },
                () =>
                {
                    Mouse.SetCursor(MouseCursor.Arrow);
                }
            );

            if (TxtInputName.IsActive)
            {
                if (TimeElapsedClick > TimeBetweenClick)
                {
                    BtnSave.Click(mouseState, () =>
                    {
                        game.player.Name = TxtInputName.Text;

                        if (NameCompleted)                        
                            UpdateScores();                        
                    });
                }
                else
                {
                    this.TimeElapsedClick += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            BtnStartWrite.Click(mouseState, () =>
            {
                TxtInputName.Activate();
            });
        }

        private void ActionGameOver(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            //Muda cursor
            BtnRestart.Hover(mouseState,
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
            if (TimeElapsedClick > TimeBetweenClick)
            {
                BtnRestart.Click(mouseState, () =>
                {
                    game.player.Score = 0;
                    game.currentScreen = new PreGameScreen(game);
                    TimeElapsedClick = 0;
                    game.currentScreen.LoadContent();                    
                });
            }
            else
            {
                TimeElapsedClick += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (PlayersScore is null)
            {
                UpdateScores(true);
                PlayersScore = FileHelper.ReadFile(game.player.CompleteFileNameScores).ToObject<List<Player>>();
            }
                
        }
        private void UpdateScores(bool update = false)
        {
            var jsonScores = string.Empty;
            var playersScore = new List<Player>();
            if (FileHelper.FileExist(game.player.CompleteFileNameScores))
            {
                jsonScores = FileHelper.ReadFile(game.player.CompleteFileNameScores);
                playersScore = jsonScores.ToObject<List<Player>>();
            }

            if (update)
            {
                var index = playersScore.FindIndex(x => x.Name.Normalization() == game.player.Name.Normalization());
                if (index > -1)                
                    playersScore[index].Score = game.player.Score;                
            }
            else
            {
                var nameExist = playersScore.Any(x => x.Name.Normalization() == game.player.Name.Normalization());
                if (!nameExist)
                {
                    playersScore.Add(game.player);
                    TimeElapsedClick = 0;
                }
                else
                {
                    game.player.Name = "";
                    TxtInputName.Text = string.Empty;
                    return;
                }
            }

            playersScore = playersScore.OrderByDescending(x => x.Score).Take(5).ToList();
            jsonScores = playersScore.ToJson();

            FileHelper.WriterFile(game.player.PathScores, game.player.FileNameScores, jsonScores);            
        }
    }

}

