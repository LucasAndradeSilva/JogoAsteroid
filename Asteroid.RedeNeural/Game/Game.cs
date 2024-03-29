﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asteroid.Gui.Models.Screens;
using Asteroid.Gui.Models.Elements;
using Asteroid.Gui.Guis;
using Asteroid.Gui.Models.Characters;
using Asteroid.Gui.Enuns;
using Asteroid.Gui.Helpers;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Microsoft.ML;
using Microsoft.ML.Data;
using Asteroid.Gui.Models.Characters.Game;
using Asteroid.Gui.Models.Characters.Nave;

namespace Asteroid.RedeNeural.Game
{
    public class Game : Screen
    {
        int TimeBetweenTraining = 100;
        int TimeElpasendTraining = 0;

        #region Elements        
        Background Background;
        Text TxtScore;
        #endregion

        #region Characters
        public Nave Nave;
        public Nave Boss;
        public List<Nave> NavesEnemy = new List<Nave>();
        public AsteroidRock AsteroidRock;
        #endregion

        #region Textures
        Texture2D NaveTexture;
        Texture2D EnemyTexture;
        Texture2D HitNaveTexture;
        Texture2D HitEnemyTexture;
        Texture2D LifeTexture;
        Texture2D BossTexture
        {
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

        public Game(AsteroidGame game) : base(game)
        {
            game.Window.Title = "Asteroid Game";

            maxRock = game.IsMobile ? 3 : 6;
            maxEnemyShips = game.IsMobile ? 3 : 6;
            limitEnemyShips = game.IsMobile ? 6 : 12;

            var SizeNave = game.IsMobile ? 100 : 64;
            var SizeLife = game.IsMobile ? 45 : 15;
            var WidthBullet = game.IsMobile ? 24 : 8;
            var HigthBullet = game.IsMobile ? 48 : 16;
            var TimeBetweenShots = game.IsMobile ? 1500 : 300;
            var SpeedShoot = game.IsMobile ? 15 : 8;
            Nave = new Nave()
            {
                Width = SizeNave,
                Heigth = SizeNave,
                Speed = 5,
                Size = SizeNave,
                Y = game.graphics.PreferredBackBufferHeight / 2,
                X = game.graphics.PreferredBackBufferWidth / 2,
                Life = new Life()
                {
                    Y = 10,
                    X = game.graphics.PreferredBackBufferWidth,
                    Width = SizeLife,
                    Heigth = SizeLife,
                    Texture = game.Content.Load<Texture2D>("Images/life")
                },
                Bullet = new Bullet()
                {
                    Speed = SpeedShoot,
                    Width = WidthBullet,
                    Heigth = HigthBullet,
                    TimeBetweenShots = TimeBetweenShots,
                    Bullets = new List<Bullet>(),
                },
                TextureName = "Images/foguete"
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
            AsteroidRock.Texture = game.Content.Load<Texture2D>("Images/asteroid");
            Nave.Texture = NaveTexture;
            Nave.Bullet.Texture = game.Content.Load<Texture2D>("Images/tiro");

            Background.Texture = game.Content.Load<Texture2D>("Images/fundo1");
            TxtScore.SpriteFont = game.Content.Load<SpriteFont>("fontes/titulo");

            HitNaveTexture = game.Content.Load<Texture2D>("Images/hitNave");

            HitEnemyTexture = game.Content.Load<Texture2D>("Images/inimigaHit");

            LifeTexture = game.Content.Load<Texture2D>("Images/life");
            EnemyTexture = game.Content.Load<Texture2D>("Images/inimiga");
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var touchState = TouchPanel.GetState().FirstOrDefault();

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

                            Nave.Life.Lifes.Remove(Nave.Life.Lifes.LastOrDefault());
                            Nave.Hit(HitNaveTexture);

                            if (Nave.Life.Lifes.Count <= 0)
                                GameOver();
                        }
                    });

                    if (enemy.CheckCollision(Nave.Rectangle))
                        GameOver();
                });
            }

            //Verifica se boss me atingiu
            if (Boss is not null)
            {
                Boss.CheckUnhit(BossTexture, gameTime.ElapsedGameTime);
                Boss.AutoMovement(game.graphics, gameTime.ElapsedGameTime);
                Boss.Bullet.AutoBulletShoot(gameTime.ElapsedGameTime, Boss);
                Boss.Bullet.BulletShootMovement(game.graphics, EnumMovement.Down, (obj) =>
                {
                    var bullet = obj as Bullet;

                    if (bullet.CheckCollision(Nave.Rectangle))
                    {
                        Boss.Bullet.Bullets.Remove(bullet);

                        Nave.Life.Lifes.Remove(Nave.Life.Lifes.LastOrDefault());
                        Nave.Hit(HitNaveTexture);

                        if (Nave.Life.Lifes.Count <= 0)
                            GameOver();
                    }
                });

                if (Boss.CheckCollision(Nave.Rectangle))
                    GameOver();
            }

            var nearestRock = AsteroidRock.Asteroids.Where(rock => Nave.CheckAreaNext(rock.Rectangle)).OrderBy(x => (x.Vector - Nave.Vector).Length())?.FirstOrDefault();
            if (nearestRock != null)
            {
                var positionRock = Nave.PositionForMe(nearestRock.Rectangle);
                if (positionRock == EnumMovement.Right)
                {
                    Nave.X -= 40;
                }
                else if (positionRock == EnumMovement.Left)
                {
                    Nave.X += 40;
                }
                else if (positionRock == EnumMovement.Up)
                {
                    var randDirection = Random.Shared.Next(0, 2);
                    if (randDirection == 0)
                        Nave.X -= 40;
                    else
                        Nave.X += 40;
                }
            }

            Nave.ScreenLimit(game.graphics);

        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Desenha o fundo
            spriteBatch.DrawElement(Background);

            // Desenha o jogador
            NaveTexture = game.Content.Load<Texture2D>(Nave.TextureName);
            Nave.Texture = NaveTexture;
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
            game.currentScreen = new Game(game);
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
                            var BulletEnemyWidth = game.IsMobile ? Random.Shared.Next(16, 32) : 8;
                            var BulletEnemyHigth = game.IsMobile ? Random.Shared.Next(32, 62) : 16;
                            var SpeedShoot = game.IsMobile ? 20 : 8;

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
                                    Speed = SpeedShoot,
                                    Width = BulletEnemyWidth,
                                    Heigth = BulletEnemyHigth,
                                    TimeBetweenShots = 1000,
                                    Bullets = new List<Bullet>(),
                                },
                                Enemy = true,
                                Texture = EnemyTexture
                            };


                            naveEnemy.Size = game.IsMobile ? Random.Shared.Next(100, 150) : Random.Shared.Next(74, 94);
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

                    var BulletBossWidth = game.IsMobile ? Random.Shared.Next(44, 54) : Random.Shared.Next(24, 34);
                    var BulletBossHigth = game.IsMobile ? Random.Shared.Next(64, 74) : Random.Shared.Next(48, 68);

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
                            Width = BulletBossWidth,
                            Heigth = BulletBossHigth,
                            TimeBetweenShots = Random.Shared.Next(700, 1000),
                            Bullets = new List<Bullet>(),
                        },
                        Enemy = true,
                        IsBoss = true,
                        SpecialShoot = true,
                        TextureName = $"Images/boss{Random.Shared.Next(1, 5)}",
                    };

                    Boss.Texture = game.Content.Load<Texture2D>(Boss.TextureName);
                    Boss.Size = game.IsMobile ? Random.Shared.Next(200, 250) : Random.Shared.Next(100, 150);
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

            Background.Texture = game.Content.Load<Texture2D>($"Images/fundo{currentNumber}");
        }




    }

}

