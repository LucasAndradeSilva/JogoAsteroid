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
        AsteroidRock AsteroidRock;
        #endregion

        Random random = new Random();

        // Pontuação do jogador
        int score = 0;

        int limitSpeed = 100;


        public GameScreen(AsteroidGame game) : base(game) {
            Nave = new Nave()
            {
                Width = 64,
                Heigth = 64,
                Speed = 5,
                Size = 64,
                Y = game.graphics.PreferredBackBufferHeight / 2,
                X = game.graphics.PreferredBackBufferWidth / 2,
                TimeBetweenShots = 200,
                Bullet = new Bullet()
                {
                    Speed = 8,
                    Width = 8,
                    Heigth = 16
                },
                Bullets = new List<Bullet>(),
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

            // Movimento do jogador
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Nave.X -= Nave.Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                Nave.X += Nave.Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                Nave.Y -= Nave.Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                Nave.Y += Nave.Speed;
            }

            // Limita o movimento do jogador dentro da tela
            Nave.X = MathHelper.Clamp(Nave.X, 0, game.graphics.PreferredBackBufferWidth - Nave.Width);
            Nave.Y = MathHelper.Clamp(Nave.Y, 0, game.graphics.PreferredBackBufferHeight - Nave.Heigth);

            // Cria novos asteroides aleatoriamente
            if (random.Next(100) < AsteroidRock.Count)
            {
                AsteroidRock.Asteroids.Add(new AsteroidRock()
                {
                    X = random.Next(game.graphics.PreferredBackBufferWidth - 64),
                    Y = -64,
                    Width = AsteroidRock.Size,
                    Heigth = AsteroidRock.Size
                });
            }            

            // Movimenta os asteroides
            for (int i = AsteroidRock.Asteroids.Count - 1; i >= 0; i--)
            {                                                
                AsteroidRock.Asteroids[i] = new AsteroidRock()
                {
                    X = AsteroidRock.Asteroids[i].X,
                    Y = AsteroidRock.Asteroids[i].Y + AsteroidRock.Speed,
                    Width = AsteroidRock.Size,
                    Heigth = AsteroidRock.Size
                };
                                
                if (AsteroidRock.Asteroids[i].Y > game.graphics.PreferredBackBufferHeight)
                {
                    AsteroidRock.Asteroids.RemoveAt(i);
                    i--;
                }
                // Verifica colisão do jogador com os asteroides
                else if (AsteroidRock.Asteroids[i].Rectangle.Intersects(Nave.Rectangle))
                {
                    // Game over
                    game.currentScreen = new GameOver(game);
                    game.currentScreen.LoadContent();
                    return;
                }
            }

            // Atira projéteis quando o botão esquerdo do mouse é pressionado
            if (keyboardState.IsKeyDown(Keys.Space) && Nave.ElapsedTimeSinceLastShot >= Nave.TimeBetweenShots)// && previousMouseState.LeftButton == ButtonState.Released)
            {
                // Adiciona um novo projetil na posição da nave
                Nave.Bullets.Add(new Bullet()
                {
                    X = Nave.X + Nave.Width / 2 - 4,
                    Y = Nave.Y,
                    Width = Nave.Bullet.Width,
                    Heigth = Nave.Bullet.Heigth
                });

                Nave.ElapsedTimeSinceLastShot = 0;
            }
            else
            {
                Nave.ElapsedTimeSinceLastShot += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            // Move os projéteis
            for (int i = Nave.Bullets.Count - 1; i >= 0; i--)
            {
                var acertou = false;
                Nave.Bullets[i] = new Bullet()
                {
                    X = Nave.Bullets[i].X,
                    Y = Nave.Bullets[i].Y - Nave.Bullet.Speed,
                    Width = Nave.Bullet.Width,
                    Heigth = Nave.Bullet.Heigth
                };

                // Verifica se o projétil acertou algum asteroide
                for (int j = AsteroidRock.Asteroids.Count - 1; j >= 0; j--)
                {
                    if (Nave.Bullets[i].Rectangle.Intersects(AsteroidRock.Asteroids[j].Rectangle))
                    {
                        acertou = true;
                        // Remove o asteroide e o projétil
                        AsteroidRock.Asteroids.RemoveAt(j);
                        j--;
                        Nave.Bullets.RemoveAt(i);
                        i--;

                        // Adiciona pontos
                        game.player.Score += 10;

                        if (game.player.Score > limitSpeed)
                        {
                            limitSpeed += 100;
                            AsteroidRock.Speed += 1;
                            AsteroidRock.Count += 1;
                            Nave.Speed += 1;                            
                        }

                        // Sai do loop interno
                        break;
                    }
                }

                //// Remove os projéteis que saíram da tela
                //if (bulletRectangles[i].Y < 0 && !acertou)
                //{
                //    bulletRectangles.RemoveAt(i);
                //    i--;
                //}
            }            
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
            foreach (var bullet in Nave.Bullets)
            {
                bullet.Texture = Nave.Bullet.Texture;
                spriteBatch.DrawElement(bullet);
            }

            // Desenha a pontuação
            TxtScore.Content = $"Pontuacao {game.player.Score}";
            spriteBatch.DrawText(TxtScore);
            
            game.spriteBatch = spriteBatch;
        }
    }
}
