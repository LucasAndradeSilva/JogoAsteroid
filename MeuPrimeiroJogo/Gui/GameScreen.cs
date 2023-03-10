using Asteroid.Models.Characters.Asteroid;
using Asteroid.Models.Characters.Nave;
using Asteroid.Models.Elements;
using Asteroid.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Asteroid.Models.Screens;
using Asteroid.Gui;
using System.Drawing;
using System.Diagnostics.Metrics;
using Asteroid.Models.Players;
using Asteroid.Enuns;

namespace Asteroid.Windows
{
    public class GameScreen : Screen
    {
        #region Elements        
        Background Background;
        Text TxtScore;
        #endregion

        #region Characters
        Nave Nave;
        List<Nave> NavesEnemy = new List<Nave>();
        AsteroidRock AsteroidRock;
        #endregion

        private EnumGameLevel GameLevel { get; set; }        

        public GameScreen(AsteroidGame game) : base(game) {
            Nave = new Nave()
            {
                Width = 64,
                Heigth = 64,
                Speed = 5,
                Size = 64,
                Y = game.graphics.PreferredBackBufferHeight / 2,
                X = game.graphics.PreferredBackBufferWidth / 2,
                Bullet = new Bullet()
                {
                    Speed = 8,
                    Width = 8,
                    Heigth = 16,
                    TimeBetweenShots = 300,
                    Bullets = new List<Bullet>(),
                }                
            };
            AsteroidRock = new AsteroidRock()
            {
                Count = 3,
                Speed = 4,
                Size = 44,
                Asteroids = new List<AsteroidRock>()
            };

            Background = new Background()
            {
                Width = game.graphics.PreferredBackBufferWidth,
                Heigth = game.graphics.PreferredBackBufferHeight
            };
            TxtScore = new Text
            {
                X = 10,
                Y = 10
            };
            
        }

        public override void LoadContent()
        {            
            AsteroidRock.Texture = game.Content.Load<Texture2D>("images/asteroid");
            Nave.Texture = game.Content.Load<Texture2D>("images/foguete");
            Nave.Bullet.Texture = game.Content.Load<Texture2D>("images/tiro");

            Background.Texture = game.Content.Load<Texture2D>("images/fundo");
            TxtScore.SpriteFont = game.Content.Load<SpriteFont>("fontes/titulo");
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            Nave.PlayerMovement(keyboardState, game.graphics);

            AsteroidRock.CreateAsteroid(game.graphics);

            AsteroidRock.AsteroidMovement(game.graphics, (obj) =>
            {
                var asteroid = obj as AsteroidRock;
                if (asteroid.CheckCollision(Nave.Rectangle))                
                    GameOver();                
            });

            Nave.Bullet.BulletShoot(keyboardState, gameTime.ElapsedGameTime, Nave);

            Nave.Bullet.BulletShootMovement(game.graphics, (obj) =>
            {
                var hit = false;
                var bullet = obj as Bullet;

                for (int j = AsteroidRock.Asteroids.Count - 1; j >= 0; j--)
                {
                    if (bullet.CheckCollision(AsteroidRock.Asteroids[j].Rectangle))
                    {
                        hit = true;
                        
                        AsteroidRock.Asteroids.RemoveAt(j);
                        j--;

                        Nave.Bullet.Bullets.Remove(bullet);                        
                        
                        game.player.UpdatePoints(AsteroidRock.Points);
                        
                        break;
                    }
                }
            });  
            
            UpdateDifficulty();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {            
            // Desenha o fundo
            spriteBatch.DrawElement(Background);

            // Desenha o jogador
            spriteBatch.DrawElement(Nave);

            // Desenha os asteroides
            foreach (var asteroid in AsteroidRock.Asteroids)
            {
                asteroid.Texture = AsteroidRock.Texture;
                spriteBatch.DrawElement(asteroid);
            }

            // Desenha os asteroides
            foreach (var bullet in Nave.Bullet.Bullets)
            {
                bullet.Texture = Nave.Bullet.Texture;
                spriteBatch.DrawElement(bullet);
            }

            // Desenha os asteroides
            foreach (var nave in NavesEnemy)
            {                
                spriteBatch.DrawElement(nave);
            }

            // Desenha a pontuação
            TxtScore.Content = $"Pontuacao {game.player.Score}";
            spriteBatch.DrawText(TxtScore);
            
            game.spriteBatch = spriteBatch;
        }

        private void GameOver()
        {            
            game.currentScreen = new GameOver(game);
            game.currentScreen.LoadContent();
            return;
        }
        

        private void UpdateDifficulty()
        {            
         
            switch (GameLevel)
            {
                case EnumGameLevel.Level0:
                    if (game.player.Score > 0)
                    {                        
                        AsteroidRock.Speed += 1;
                        //AsteroidRock.Count += 1;
                        NavesEnemy.Add(new Nave()
                        {
                            Width = 64,
                            Heigth = 64,
                            Speed = 5,
                            Size = 64,                                                        
                            Y = 0,
                            X = game.graphics.PreferredBackBufferWidth / 2,
                            Roatation = 180,
                            Bullet = new Bullet()
                            {
                                Speed = 8,
                                Width = 8,
                                Heigth = 16,
                                TimeBetweenShots = 300,
                                Bullets = new List<Bullet>(),
                            },
                            Enemy = true,
                            Texture = game.Content.Load<Texture2D>("images/inimiga")                            
                        });
                    }
                    break;
                case EnumGameLevel.Level1:
                    if (game.player.Score > 200)
                    {
                        AsteroidRock.Speed += 1;
                        AsteroidRock.Count += 1;
                        Nave.Speed+= 1;
                        //Naves inimigas
                    }
                    break;
                case EnumGameLevel.Level2:
                    if (game.player.Score > 350)
                    {
                        AsteroidRock.Speed += 1;
                        AsteroidRock.Count += 1;
                        //Fist boss
                    }
                    break;
                case EnumGameLevel.Level3:
                    if (game.player.Score > 450)
                    {
                        AsteroidRock.Speed += 1;
                        AsteroidRock.Count += 1;
                        //Naves inimigas
                    }
                    break;
                case EnumGameLevel.Level4:
                    if (game.player.Score > 600)
                    {
                        AsteroidRock.Speed += 1;
                        AsteroidRock.Count += 1;
                        Nave.Speed += 1;
                        //boss
                    }
                    break;
                case EnumGameLevel.Level5:
                    if (game.player.Score > 750)
                    {
                        AsteroidRock.Speed += 1;
                        AsteroidRock.Count += 1;                        
                        //Naves inimigas
                    }
                    break;
                case EnumGameLevel.Level6:
                    if (game.player.Score > 1000)
                    {
                        AsteroidRock.Speed += 1;
                        AsteroidRock.Count += 1;
                        //ultimate boss
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
