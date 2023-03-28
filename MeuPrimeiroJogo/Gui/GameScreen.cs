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
using System.Threading;
using Asteroid.Models.Characters;
using System.Linq;

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
		Nave Boss;
        List<Nave> NavesEnemy = new List<Nave>();
        AsteroidRock AsteroidRock;
        #endregion


        #region Difficulty        
		private int nextDifficultyScore = 50;
		private int maxEnemyShips = 12;		
        #endregion

        public GameScreen(AsteroidGame game) : base(game) {
			game.Window.Title = "Asteroid Game";

			Nave = new Nave()
            {
                Width = 64,
                Heigth = 64,
                Speed = 5,
                Size = 64,
                Y = game.graphics.PreferredBackBufferHeight / 2,
                X = game.graphics.PreferredBackBufferWidth / 2,
                Life = new Life()
                {
                    Y = 10,
                    X = game.graphics.PreferredBackBufferWidth,
                    Width = 15,
                    Heigth = 15,                                        
                    Texture = game.Content.Load<Texture2D>("images/life")
                },
                Bullet = new Bullet()
                {
                    Speed = 8,
                    Width = 8,
                    Heigth = 16,
                    TimeBetweenShots = 300,
                    Bullets = new List<Bullet>(),
                }                
            };

            Nave.Life.CreateLifes(4);

            AsteroidRock = new AsteroidRock()
            {
                Count = 3,
                Speed = 1,
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
                if (asteroid.CheckCollision(Nave.Rectangle) && !asteroid.Destroyed)
                {
                    AsteroidRock.Asteroids.Remove(asteroid);

                    Nave.Life.Lifes.RemoveAt(0);

                    if (Nave.Life.Lifes.Count <= 0)
                        GameOver();
                }
            });

            Nave.Bullet.BulletShoot(keyboardState, gameTime.ElapsedGameTime, Nave);

            Nave.Bullet.BulletShootMovement(game.graphics, EnumMovement.Up, (obj) =>
            {
                var bullet = obj as Bullet;

                //Virifica se atingiu asteroid
                for (int j = AsteroidRock.Asteroids.Count - 1; j >= 0; j--)
                {
                    if (bullet.CheckCollision(AsteroidRock.Asteroids[j].Rectangle) && !AsteroidRock.Asteroids[j].Destroyed)
                    {                                                
                        AsteroidRock.Asteroids[j].Texture = game.Content.Load<Texture2D>("images/explosao");
                        AsteroidRock.Asteroids[j].Destroyed = true;                        

                        Nave.Bullet.Bullets.Remove(bullet);

                        game.player.UpdatePoints(AsteroidRock.Asteroids[j].Points);
                        
                        break;
                    }
                }

                //Verifica se atingiu Nava inimiga
                for (int i = NavesEnemy.Count - 1; i >= 0; i--)
                {
                    if (bullet.CheckCollision(NavesEnemy[i].Rectangle))
                    {                        
                        NavesEnemy[i].Life.Lifes.RemoveAt(0);

                        if (NavesEnemy[i].Life.Lifes.Count <= 0)
                        {
                            game.player.UpdatePoints(NavesEnemy[i].Points);
                            NavesEnemy.RemoveAt(i);                            
                        }

						Nave.Bullet.Bullets.Remove(bullet);

						break;
                    }
                }

                //Verifica se atingiu boss
                if (Boss is not null)
                {                    
					if (bullet.CheckCollision(Boss.Rectangle))
                    {
                        Boss.Life.Lifes.RemoveAt(0);

						if (Boss.Life.Lifes.Count <= 0)
						{
							game.player.UpdatePoints(Boss.Points);
						
							Nave.Bullet.Bullets.Remove(bullet);

                            Boss = null;

                            ResetAsteroids();                            
						}

						Nave.Bullet.Bullets.Remove(bullet);                       
					}
                }
            });

            //Verifica se o tiro da nave inimiga me acertou
            if (NavesEnemy.Count > 0)
            {
                NavesEnemy.ForEach((enemy) =>
                {
                    enemy.AutoMovement(game.graphics, gameTime.ElapsedGameTime);
                    enemy.Bullet.AutoBulletShoot(gameTime.ElapsedGameTime, enemy);
                    enemy.Bullet.BulletShootMovement(game.graphics, EnumMovement.Down, (obj) =>
                    {
                        var hit = false;
                        var bullet = obj as Bullet;

                        if (bullet.CheckCollision(Nave.Rectangle))
                        {
                            Nave.Life.Lifes.RemoveAt(0);

                            if (Nave.Life.Lifes.Count <= 0)                         
                                GameOver();                            
                        }                       
                    });
                });
            }

            //Verifica se boss me atingiu
            if (Boss is not null)
            {
                Boss.AutoMovement(game.graphics, gameTime.ElapsedGameTime);
				Boss.Bullet.AutoBulletShoot(gameTime.ElapsedGameTime, Boss);
				Boss.Bullet.BulletShootMovement(game.graphics, EnumMovement.Down, (obj) =>
				{
					var hit = false;
					var bullet = obj as Bullet;

					if (bullet.CheckCollision(Nave.Rectangle))
					{
						Nave.Life.Lifes.RemoveAt(0);

						if (Nave.Life.Lifes.Count <= 0)
							GameOver();
					}
				});
			}

			UpdateDifficulty();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {            
            // Desenha o fundo
            spriteBatch.DrawElement(Background);

            // Desenha o jogador
            spriteBatch.DrawElement(Nave);

			// Desenha boss
			if (Boss is not null)
            {
				spriteBatch.DrawElement(Boss);
                foreach (var bullet in Boss.Bullet.Bullets)
                {
					bullet.Texture = Nave.Bullet.Texture;
					spriteBatch.DrawElement(bullet);
				}
			}				
            
            // Desenha vidas
            var naveBase = Nave.Life.Lifes.FirstOrDefault();
            for (int i = 0; i < Nave.Life.Lifes.Count; i++)
            {
                if (!Nave.Life.Lifes[i].FisrtDraw)
                {                      
                    naveBase.X -= 20; 
                    Nave.Life.Lifes[i].X = naveBase.X;
                    Nave.Life.Lifes[i].FisrtDraw = true;
                }                    

                spriteBatch.DrawElement(Nave.Life.Lifes[i]);
            }            
            
            // Desenha os asteroides                        
            foreach (var asteroid in AsteroidRock.Asteroids)
            {
                //asteroid.Texture = AsteroidRock.Texture;
                spriteBatch.DrawElement(asteroid);
            }

            // Desenha os tiros
            foreach (var bullet in Nave.Bullet.Bullets)
            {
                bullet.Texture = Nave.Bullet.Texture;
                spriteBatch.DrawElement(bullet);
            }

            // Desenha as naves inimigas
            foreach (var nave in NavesEnemy)
            {                
                spriteBatch.DrawElement(nave);

                foreach (var bullet in nave.Bullet.Bullets)
                {
                    bullet.Texture = Nave.Bullet.Texture;
                    spriteBatch.DrawElement(bullet);
                }
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
			if (game.player.Score > nextDifficultyScore)
			{				
				nextDifficultyScore += Random.Shared.Next(50, 150);
				
				AsteroidRock.Count = Random.Shared.Next(3, 6);
				
				if (NavesEnemy.Count < maxEnemyShips)
				{                   
					if (Random.Shared.Next(0, 2) == 0)
					{
                        var navesQtd = Random.Shared.Next(1, 6);
                        for (int i = 0; i < navesQtd; i++)
                        {
							var naveEnemy = new Nave()
							{
								Points = Random.Shared.Next(25, 50),
								TimeBetweenMovement = Random.Shared.Next(1000, 3000),								
								Speed = Random.Shared.Next(1, 3),								
								Y = 0,
								X = Random.Shared.Next(game.graphics.PreferredBackBufferWidth),
								Roatation = 180,
								Life = new Life()
								{
									Texture = game.Content.Load<Texture2D>("images/life")
								},
								Bullet = new Bullet()
								{
									Speed = 8,
									Width = 8,
									Heigth = 16,
									TimeBetweenShots = 1000,
									Bullets = new List<Bullet>(),
								},
								Enemy = true,
								Texture = game.Content.Load<Texture2D>("images/inimiga")
							};

                            naveEnemy.Size = Random.Shared.Next(64, 84);
                            naveEnemy.Width = naveEnemy.Size;
							naveEnemy.Heigth = naveEnemy.Size;
							naveEnemy.Life.CreateLifes(Random.Shared.Next(1, 3));

							NavesEnemy.Add(naveEnemy);
						}						
					}
				}

                //Random boss
                if (Random.Shared.Next(0, 10) == 0 && Boss is null)
                {
                    ClearAsteroids();                    

					Boss = new Nave()
					{
						Points = Random.Shared.Next(50, 120),
						TimeBetweenMovement = Random.Shared.Next(1500, 2000),						
						Speed = Random.Shared.Next(3, 5),						
						Y = 0,
						X = Random.Shared.Next(game.graphics.PreferredBackBufferWidth),
						Roatation = 180,
						Life = new Life()
						{
							Texture = game.Content.Load<Texture2D>("images/life")
						},
						Bullet = new Bullet()
						{
							Speed = 8,
							Width = 8,
							Heigth = 16,
							TimeBetweenShots = 700,
							Bullets = new List<Bullet>(),
						},
						Enemy = true,
						Texture = game.Content.Load<Texture2D>($"images/boss{Random.Shared.Next(1, 5)}")
					};

					Boss.Size = Random.Shared.Next(100, 150);
					Boss.Width = Boss.Size;
					Boss.Heigth = Boss.Size;

					Boss.Life.CreateLifes(Random.Shared.Next(10, 15));
				}
			}		
		}

		private void ClearAsteroids()
		{
			AsteroidRock.Asteroids.Clear();
			AsteroidRock.Count = -1;
		}
		private void ResetAsteroids()
		{
			AsteroidRock.Count = 3;
		}
		private void ClearNavesEnemy()
        {
            NavesEnemy.Clear();
			maxEnemyShips = 0;
		}

        private void ResetNavesEnemy()
        {
			maxEnemyShips = 6;
		}
    }
}
