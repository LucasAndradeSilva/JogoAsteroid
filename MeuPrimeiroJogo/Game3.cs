using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AsteroidsGame
{
    public class Game3 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D backgroundTexture;
        Texture2D playerTexture;
        Texture2D asteroidTexture;
        Texture2D bulletTexture;
        SpriteFont font;

        Rectangle playerRectangle;
        List<Rectangle> asteroidRectangles;
        List<Rectangle> bulletRectangles;
        Random random = new Random();
        float playerSpeed = 5f;
        // Velocidade dos asteroides em pixels por segundo
        int asteroidSpeed = 4;

        // Tamanho dos asteroides em pixels
        int asteroidSize = 64;

        // Quantidade de asteroides que aparecem na tela ao mesmo tempo
        int asteroidCount = 3;

        // Velocidade das balas em pixels por segundo
        int bulletSpeed = 8;

        // Tempo mínimo entre disparos em milissegundos
        int minTimeBetweenShots = 200;

        // Tempo desde o último disparo em milissegundos
        int timeSinceLastShot = 0;

        // Pontuação do jogador
        int score = 0;

        int limitSpeed = 100;


        public Game3()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            playerRectangle = new Rectangle(0, 0, 64, 64);
            asteroidRectangles = new List<Rectangle>();
            bulletRectangles = new List<Rectangle>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTexture = Content.Load<Texture2D>("images/fundo");
            playerTexture = Content.Load<Texture2D>("images/foguete");
            asteroidTexture = Content.Load<Texture2D>("images/asteroid");
            bulletTexture = Content.Load<Texture2D>("images/tiro");
            font = Content.Load<SpriteFont>("fontes/arial");

        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            // Movimento do jogador
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                playerRectangle.X -= (int)playerSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                playerRectangle.X += (int)playerSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                playerRectangle.Y -= (int)playerSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                playerRectangle.Y += (int)playerSpeed;
            }

            // Limita o movimento do jogador dentro da tela
            playerRectangle.X = MathHelper.Clamp(playerRectangle.X, 0, graphics.PreferredBackBufferWidth - playerRectangle.Width);
            playerRectangle.Y = MathHelper.Clamp(playerRectangle.Y, 0, graphics.PreferredBackBufferHeight - playerRectangle.Height);

            // Cria novos asteroides aleatoriamente
            if (random.Next(100) < 2)
            {
                asteroidRectangles.Add(new Rectangle(random.Next(graphics.PreferredBackBufferWidth - 64), -64, 64, 64));
            }

            // Movimenta os asteroides
            for (int i = asteroidRectangles.Count - 1; i >= 0; i--)
            {
                asteroidRectangles[i] = new Rectangle(asteroidRectangles[i].X, asteroidRectangles[i].Y + asteroidSpeed, asteroidSize, asteroidSize);
                if (asteroidRectangles[i].Y > graphics.PreferredBackBufferHeight)
                {
                    asteroidRectangles.RemoveAt(i);
                    i--;
                }
                // Verifica colisão do jogador com os asteroides
                else if (asteroidRectangles[i].Intersects(playerRectangle))
                {
                    // Game over
                    Exit();
                }
            }

            // Atira projéteis quando o botão esquerdo do mouse é pressionado
            if (keyboardState.IsKeyDown(Keys.Space))// && previousMouseState.LeftButton == ButtonState.Released)
            {
                // Adiciona um novo projetil na posição do jogador
                bulletRectangles.Add(new Rectangle(playerRectangle.X + playerRectangle.Width / 2 - 4, playerRectangle.Y, 8, 16));
            }

            // Move os projéteis
            for (int i = bulletRectangles.Count - 1; i >= 0; i--)
            {
                bulletRectangles[i] = new Rectangle(bulletRectangles[i].X, bulletRectangles[i].Y - bulletSpeed, 8, 16);

                // Verifica se o projétil acertou algum asteroide
                for (int j = asteroidRectangles.Count - 1; j >= 0; j--)
                {
                    if (bulletRectangles[i].Intersects(asteroidRectangles[j]))
                    {
                        // Remove o asteroide e o projétil
                        asteroidRectangles.RemoveAt(j);
                        j--;
                        bulletRectangles.RemoveAt(i);
                        i--;

                        // Adiciona pontos
                        score += 10;

                        if (score > limitSpeed)
                        {
                            limitSpeed += 100;
                            asteroidSpeed += 1;
                            playerSpeed += 1;
                        }

                        // Sai do loop interno
                        break;
                    }
                }

                // Remove os projéteis que saíram da tela
                //if (bulletRectangles[i].Y < 0)
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
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            // Desenha o jogador
            spriteBatch.Draw(playerTexture, playerRectangle, Color.White);

            // Desenha os asteroides
            foreach (Rectangle asteroidRectangle in asteroidRectangles)
            {
                spriteBatch.Draw(asteroidTexture, asteroidRectangle, Color.White);
            }

            // Desenha os asteroides
            foreach (Rectangle bulletRectangle in bulletRectangles)
            {
                spriteBatch.Draw(bulletTexture, bulletRectangle, Color.White);
            }

            // Desenha a pontuação
            spriteBatch.DrawString(font, $"Pontuacao: {score}", new Vector2(10, 10), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
