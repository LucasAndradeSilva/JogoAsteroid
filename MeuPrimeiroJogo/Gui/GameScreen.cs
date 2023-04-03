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
using Asteroid.Models.Characters.Game;

namespace Asteroid.Gui
{
    public class GameScreen : Screen
    {
        #region Elements        
        Background Background;
        Text TxtScore;
        #endregion

        #region Characters
        public Nave Nave;
		public Nave Boss;
        public List<Nave> NavesEnemy = new List<Nave>();
        public List<PowerUp> PowerUps = new List<PowerUp>();
        public AsteroidRock AsteroidRock;
        #endregion

        #region Textures
        Texture2D NaveTexture;
        Texture2D EnemyTexture;
        Texture2D HitNaveTexture;        
        Texture2D HitEnemyTexture;
        Texture2D LifeTexture;
        Texture2D BossTexture { 
            get
            {
                return game.Content.Load<Texture2D>(Boss.TextureName);
            }
        }
        #endregion

        #region Difficulty        
        private int nextDifficultyScore = 50;
		private int limitEnemyShips = 12; 
        private int maxEnemyShips = 6;
        private int maxRock = 6;
        int lastBackgroundNumber = 0;

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
                },
                Powers = new List<PowerUp>(),
                TextureName = "images/foguete"
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
            NaveTexture = game.Content.Load<Texture2D>(Nave.TextureName);
            AsteroidRock.Texture = game.Content.Load<Texture2D>("images/asteroid");
            Nave.Texture = NaveTexture;
            Nave.Bullet.Texture = game.Content.Load<Texture2D>("images/tiro");

            Background.Texture = game.Content.Load<Texture2D>("images/fundo1");
            TxtScore.SpriteFont = game.Content.Load<SpriteFont>("fontes/titulo");

            HitNaveTexture = game.Content.Load<Texture2D>("images/hitNave");

            HitEnemyTexture = game.Content.Load<Texture2D>("images/inimigaHit");

            LifeTexture = game.Content.Load<Texture2D>("images/life");
            EnemyTexture = game.Content.Load<Texture2D>("images/inimiga");
        }
        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            Nave.Initialize(keyboardState, game.graphics, this, NaveTexture, gameTime);            
            
            AsteroidRock.CreateAsteroid(game.graphics);

            AsteroidRock.AsteroidMovement(game.graphics, (obj) =>
            {
                var asteroid = obj as AsteroidRock;
                if (asteroid.CheckCollision(Nave.Rectangle) && !asteroid.Destroyed)
                {
                    AsteroidRock.Asteroids.Remove(asteroid);

                    if (!Nave.Immune)
                    {
                        Nave.Life.Lifes.Remove(Nave.Life.Lifes.LastOrDefault());
                        Nave.Hit(HitNaveTexture);

                        if (Nave.Life.Lifes.Count <= 0)
                            GameOver();
                    }                    
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
                        NavesEnemy[i].Hit(HitEnemyTexture);

                        if (NavesEnemy[i].Life.Lifes.Count <= 0)
                        {
                            game.player.UpdatePoints(NavesEnemy[i].Points);
                            CheckPowerUp(NavesEnemy[i].Rectangle);
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
                        Boss.Hit(game.Content.Load<Texture2D>(Boss.GetTextureHit()));

						if (Boss.Life.Lifes.Count <= 0)
						{
							game.player.UpdatePoints(Boss.Points);
						
							Nave.Bullet.Bullets.Remove(bullet);
                           
                            CheckPowerUp(Boss.Rectangle);
                            ResetAsteroids();
                            UpdateHardDiffculty();

                            Boss = default;
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
                    enemy.CheckUnhit(EnemyTexture, gameTime.ElapsedGameTime);
                    enemy.AutoMovement(game.graphics, gameTime.ElapsedGameTime);
                    enemy.Bullet.AutoBulletShoot(gameTime.ElapsedGameTime, enemy);
                    enemy.Bullet.BulletShootMovement(game.graphics, EnumMovement.Down, (obj) =>
                    {                        
                        var bullet = obj as Bullet;

                        if (bullet.CheckCollision(Nave.Rectangle))
                        {
                            enemy.Bullet.Bullets.Remove(bullet);

                            if (!Nave.Immune)
                            {
                                Nave.Life.Lifes.Remove(Nave.Life.Lifes.LastOrDefault());
                                Nave.Hit(HitNaveTexture);

                                if (Nave.Life.Lifes.Count <= 0)
                                    GameOver();
                            }
                        }                       
                    });
                });
            }

            //Verifica se boss me atingiu
            if (Boss is not null)
            {                
                Boss.CheckUnhit(BossTexture, gameTime.ElapsedGameTime);
                Boss.AutoMovement(game.graphics, gameTime.ElapsedGameTime);
				Boss.Bullet.AutoBulletShoot(gameTime.ElapsedGameTime, Boss);
				Boss.Bullet.BulletShootMovement(game.graphics, (obj) =>
				{					
					var bullet = obj as Bullet;

					if (bullet.CheckCollision(Nave.Rectangle))
					{
                        Boss.Bullet.Bullets.Remove(bullet);

                        if (!Nave.Immune)
                        {
                            Nave.Life.Lifes.Remove(Nave.Life.Lifes.LastOrDefault());
                            Nave.Hit(HitNaveTexture);
                         
                            if (Nave.Life.Lifes.Count <= 0)
                                GameOver();
                        }
					}
				});
			}

            if (PowerUps.Count > 0)
            {
                for (int i = 0; i < PowerUps.Count; i++)
                {
                    PowerUps[i].Moviment(EnumMovement.Down, PowerUps[i].Speed, game.graphics);

                    if (PowerUps[i].CheckCollision(Nave.Rectangle))
                    {
                        PowerUps[i].X = game.graphics.PreferredBackBufferWidth - 40;
                        PowerUps[i].Y = game.graphics.PreferredBackBufferHeight - 60;
                        Nave.Powers.Add(PowerUps[i]);

                        PowerUps.RemoveAt(0);
                    }
                    else
                    {
                        PowerUps[i].CheckLeftScreen(game.graphics, EnumMovement.Down, () =>
                        {
                            PowerUps.RemoveAt(0);
                        });
                    }
                }
            
            }

			UpdateDifficulty();
        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {            
            // Desenha o fundo
            spriteBatch.DrawElement(Background);

            // Desenha o jogador
            NaveTexture = game.Content.Load<Texture2D>(Nave.TextureName);
            Nave.Texture = NaveTexture;
            spriteBatch.DrawElement(Nave);
            for (int i = 0; i < Nave.Powers.Count; i++)
            {
                if (!Nave.Powers[i].FisrtDraw)
                {
                    var width = (i + 1) * 60;
                    Nave.Powers[i].X -= width;
                    Nave.Powers[i].FisrtDraw = true;
                }

                spriteBatch.DrawElement(Nave.Powers[i]);
            }                             

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

            // Desenha power up
            foreach (var power in PowerUps)
            {
                spriteBatch.DrawElement(power);
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
				AsteroidRock.Count = Random.Shared.Next(3, maxRock);
				
				if (NavesEnemy.Count < limitEnemyShips)
				{                   
					if (Random.Shared.Next(0, 2) == 0)
					{
                        var navesQtd = Random.Shared.Next(1, maxEnemyShips);
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
									Texture = LifeTexture
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
								Texture = EnemyTexture
                            };

                            naveEnemy.Size = Random.Shared.Next(74, 94);
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
                    UpdateBackgraund();
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
                            Texture = LifeTexture
                        },
                        Bullet = new Bullet()
                        {
                            Speed = 8,
                            Width = Random.Shared.Next(10, 16),
                            Heigth = Random.Shared.Next(16, 24),
                            TimeBetweenShots = Random.Shared.Next(700, 1000),
                            Bullets = new List<Bullet>(),
                        },
                        Enemy = true,
                        IsBoss = true,
                        TextureName = $"images/boss{Random.Shared.Next(1, 5)}",                        
					};

                    Boss.Texture = game.Content.Load<Texture2D>(Boss.TextureName);
                    Boss.Size = Random.Shared.Next(100, 150);
					Boss.Width = Boss.Size;
					Boss.Heigth = Boss.Size;

					Boss.Life.CreateLifes(Random.Shared.Next(15, 20));
				}
			}		
		}
        private void UpdateHardDiffculty()
        {
            var random = Random.Shared.Next(1, 10);
            if (random == 1)
            {
                maxRock += 1;
            }

            random = Random.Shared.Next(1, 10);
            if (random == 1)
            {
                maxEnemyShips += 1;
                limitEnemyShips = 1;
            }
        }
		public void ClearAsteroids()
		{
			AsteroidRock.Asteroids.Clear();
			AsteroidRock.Count = -1;
		}
		public void ResetAsteroids()
		{
			AsteroidRock.Count = 3;
		}
        public void ClearNavesEnemy()
        {
            NavesEnemy.Clear();
			maxEnemyShips = 0;
		}
        public void ResetNavesEnemy()
        {
			maxEnemyShips = 6;
		}
        private void UpdateBackgraund()
        {
            var currentNumber = Random.Shared.Next(1, 9);

            while (lastBackgroundNumber == currentNumber)
            {
                currentNumber = Random.Shared.Next(1, 9);
            }

            lastBackgroundNumber = currentNumber;

            Background.Texture = game.Content.Load<Texture2D>($"images/fundo{currentNumber}");
        }
        private void CheckPowerUp(Microsoft.Xna.Framework.Rectangle position)
        {
            var powerup = PowerUp.GeneretPowerUp();
            if (powerup is not null)
            {
                powerup.X = position.X;
                powerup.Y = position.Y;
                powerup.Texture = game.Content.Load<Texture2D>(powerup.TextureName);
                PowerUps.Add(powerup);
            }                
        }
    }
}
