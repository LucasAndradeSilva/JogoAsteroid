using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AsteroidsGame
{
    public class Game2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D backgroundTexture;
        Texture2D playerTexture;
        Texture2D asteroidTexture;
        Rectangle playerRectangle;
        List<Rectangle> asteroidRectangles;
        Random random = new Random();
        float playerSpeed = 5f;
        // Velocidade dos asteroides em pixels por segundo
        int asteroidSpeed = 4;

        // Tamanho dos asteroides em pixels
        int asteroidSize = 64;

        // Quantidade de asteroides que aparecem na tela ao mesmo tempo
        int asteroidCount = 3;

        public Game2()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            playerRectangle = new Rectangle(0, 0, 64, 64);
            asteroidRectangles = new List<Rectangle>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTexture = Content.Load<Texture2D>("images/fundo");
            playerTexture = Content.Load<Texture2D>("images/foguete");
            asteroidTexture = Content.Load<Texture2D>("images/asteroid");
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
