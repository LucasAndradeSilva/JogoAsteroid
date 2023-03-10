using Asteroid.Enuns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Asteroid.Models.Elements
{
    public abstract class Element {
        public Rectangle Rectangle {
            get
            {
                var rectangle = new Rectangle(X, Y, Width, Heigth);                
                return rectangle;
            }
        }
        public Vector2 Vector
        {
            get
            {
                var vector = new Vector2(X, Y);
                return vector;
            }
        }

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 0; 
        public int Heigth { get; set; } = 0;
        public float Scale { get; set; } = 1;
        public float LayerDepth { get; set; }
        public float Roatation { get; set; }
        public Color Color { get; set; } = Color.White;
        public Vector2 Origin { get; set; } = Vector2.Zero;        
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
        public Texture2D Texture { get; set; }
        public SpriteFont SpriteFont { get; set; }

        public void Click(MouseState mouseState, Action action)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && this.Rectangle.Contains(mouseState.Position))
            {
                action();
            }
        }

        public bool IsHover(MouseState mouseState)
        {
            return this.Rectangle.Contains(mouseState.Position);
        }

        public void Hover(MouseState mouseState, Action PositiveAction, Action NegativeAction)
        {
            if (this.Rectangle.Contains(mouseState.Position))
                PositiveAction();
            else
                NegativeAction();
        }

        public void Moviment(EnumMovement movement, int Speed, GraphicsDeviceManager graphics)
        {
            switch (movement)
            {
                case EnumMovement.Down:
                    this.Y += Speed;
                    break;
                case EnumMovement.Up:
                    this.Y -= Speed;
                    break;
                case EnumMovement.Left:
                    this.X -= Speed;
                    break;
                case EnumMovement.Right:
                    this.X += Speed;
                    break;
                case EnumMovement.LeftDown:
                    this.X -= Speed;
                    this.Y += Speed;
                    break;
                case EnumMovement.RightDown:
                    this.X += Speed;
                    this.Y += Speed;
                    break;
                case EnumMovement.LeftUp:
                    this.X -= Speed;
                    this.Y -= Speed;
                    break;
                case EnumMovement.RightUp:
                    this.X += Speed;
                    this.Y -= Speed;
                    break;
                default:
                    break;
            }            
        }

        public void ScreenLimit(GraphicsDeviceManager graphics)
        {
            this.X = MathHelper.Clamp(this.X, 0, graphics.PreferredBackBufferWidth - this.Width);
            this.Y = MathHelper.Clamp(this.Y, 0, graphics.PreferredBackBufferHeight - this.Heigth);
        }

        public bool CheckLeftScreen(GraphicsDeviceManager graphics, EnumMovement movement,  Action CallBackAction)
        {
            if (movement == EnumMovement.Down) 
            {
                if (this.Y > graphics.PreferredBackBufferHeight)
                {

                    CallBackAction();
                    return true;
                }
            }
            else if (movement == EnumMovement.Up)
            {
                if (this.Y <= 0)
                {
                    CallBackAction();
                    return true;
                }
            }

            return false;
        }
    }
}
