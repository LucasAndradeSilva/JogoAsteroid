using Asteroid.Dao.Characters.Asteroid;
using Asteroid.Dao.Characters.Nave;
using Asteroid.Dao.Elements;
using Asteroid.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Asteroid.Windows
{
    public class GameScreen : Microsoft.Xna.Framework.Game
    {
        #region Elements
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
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


        public GameScreen()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Nave = new Nave()
            {
                Width = 64,
                Heigth = 64,
                Speed = 5,
                Size = 64,
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
                Width = graphics.PreferredBackBufferWidth,
                Heigth = graphics.PreferredBackBufferHeight
            };
            TxtScore = new Text
            {
                X = 10,
                Y = 10
            };            
                        
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AsteroidRock.Texture = Content.Load<Texture2D>("images/asteroid");
            Nave.Texture = Content.Load<Texture2D>("images/foguete");
            Nave.Bullet.Texture = Content.Load<Texture2D>("images/tiro");

            Background.Texture = Content.Load<Texture2D>("images/fundo");
            TxtScore.SpriteFont = Content.Load<SpriteFont>("fontes/titulo");           
        }

        protected override void Update(GameTime gameTime)
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
            Nave.X = MathHelper.Clamp(Nave.X, 0, graphics.PreferredBackBufferWidth - Nave.Width);
            Nave.Y = MathHelper.Clamp(Nave.Y, 0, graphics.PreferredBackBufferHeight - Nave.Heigth);

            // Cria novos asteroides aleatoriamente
            if (random.Next(100) < AsteroidRock.Count)
            {
                AsteroidRock.Asteroids.Add(new AsteroidRock()
                {
                    X = random.Next(graphics.PreferredBackBufferWidth - 64),
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

                if (AsteroidRock.Asteroids[i].Y > graphics.PreferredBackBufferHeight)
                {
                    AsteroidRock.Asteroids.RemoveAt(i);
                    i--;
                }
                // Verifica colisão do jogador com os asteroides
                else if (AsteroidRock.Asteroids[i].Rectangle.Intersects(Nave.Rectangle))
                {
                    // Game over
                    Exit();
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
                        score += 10;

                        if (score > limitSpeed)
                        {
                            limitSpeed += 100;
                            AsteroidRock.Speed += 1;
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

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
            TxtScore.Content = $"Pontuacao {score}";
            spriteBatch.DrawText(TxtScore);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
