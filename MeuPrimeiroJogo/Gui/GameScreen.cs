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
        List<Nave> NavesEnemy = new List<Nave>();
        AsteroidRock AsteroidRock;
        #endregion


        #region Difficulty
        private int LimitNaves = 0;
        private EnumGameLevel GameLevel { get; set; }

        #endregion

        public GameScreen(AsteroidGame game) : base(game) {
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

            Nave.Life.CreateLifes(3);

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

                for (int j = AsteroidRock.Asteroids.Count - 1; j >= 0; j--)
                {
                    if (bullet.CheckCollision(AsteroidRock.Asteroids[j].Rectangle) && !AsteroidRock.Asteroids[j].Destroyed)
                    {                                                
                        AsteroidRock.Asteroids[j].Texture = game.Content.Load<Texture2D>("images/explosao");
                        AsteroidRock.Asteroids[j].Destroyed = true;                        

                        Nave.Bullet.Bullets.Remove(bullet);                        
                        
                        game.player.UpdatePoints(AsteroidRock.Points);
                        
                        break;
                    }
                }

                for (int i = NavesEnemy.Count - 1; i >= 0; i--)
                {
                    if (bullet.CheckCollision(NavesEnemy[i].Rectangle))
                    {                        
                        NavesEnemy[i].Life.Lifes.RemoveAt(0);

                        if (NavesEnemy[i].Life.Lifes.Count <= 0)
                        {
                            game.player.UpdatePoints(NavesEnemy[i].Points);
                            NavesEnemy.RemoveAt(i);
                            Nave.Bullet.Bullets.Remove(bullet);
                            break;
                        }                                                                        
                    }
                }
            });

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

            UpdateDifficulty();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {            
            // Desenha o fundo
            spriteBatch.DrawElement(Background);

            // Desenha o jogador
            spriteBatch.DrawElement(Nave);

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
         
            switch (GameLevel)
            {              
                case EnumGameLevel.Level1:
                    if (game.player.Score > 100)
                    {
                        AsteroidRock.Count = 2;                        

                        //Naves inimigas
                        if (LimitNaves <= 3)
                        {
                            var naveEnemy = new Nave()
                            {
                                Points = 25,
                                TimeBetweenMovement = 1500,
                                Width = 64,
                                Heigth = 64,
                                Speed = 1,
                                Size = 64,
                                Y = 0,
                                X = game.graphics.PreferredBackBufferWidth / 2,
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

                            naveEnemy.Life.CreateLifes(3);

                            NavesEnemy.Add(naveEnemy);
                            LimitNaves++;
                        }
                    }

                    if (game.player.Score > 400)
                    {
                        LimitNaves = 0;
                        GameLevel = EnumGameLevel.Level2;
                    }

                     break;
                case EnumGameLevel.Level2:
                    AsteroidRock.Count = 3;

                    //Naves inimigas
                    if (LimitNaves <= 5)
                        {
                            var naveEnemy = new Nave()
                            {
                                Points = 25,
                                TimeBetweenMovement = 1500,
                                Width = 64,
                                Heigth = 64,
                                Speed = 1,
                                Size = 64,
                                Y = 0,
                                X = game.graphics.PreferredBackBufferWidth / 2,
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

                            naveEnemy.Life.CreateLifes(2);

                            NavesEnemy.Add(naveEnemy);
                            LimitNaves++;
                        }                        
                   

                    if (game.player.Score > 700)
                    {
                        LimitNaves = 0;
                        GameLevel = EnumGameLevel.Level3;
                    }
                    break;
                case EnumGameLevel.Level3:
                                       
                    AsteroidRock.Count = -1;                    

                    //Bosss
                    if (LimitNaves < 1)
                        {
                            var boss = new Nave()
                            {
                                Points = 100,
                                TimeBetweenMovement = 1500,
                                Width = 128,
                                Heigth = 128,
                                Speed = 1,
                                Size = 64,
                                Y = 0,
                                X = game.graphics.PreferredBackBufferWidth / 2,
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

                            boss.Life.CreateLifes(10);

                            NavesEnemy.Add(boss);
                            LimitNaves++;
                        }

                    if (game.player.Score > 790)
                    {
                        LimitNaves = 0;
                        GameLevel = EnumGameLevel.Level4;
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
