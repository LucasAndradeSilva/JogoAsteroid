using Asteroid.Models.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Characters.Nave
{
    public class Nave : Character
    {
        public Bullet Bullet { get; set; }
        public bool Enemy { get; set; }
        public void PlayerMovement(KeyboardState keyboardState, GraphicsDeviceManager graphics)
        {
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                this.X -= this.Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                this.X += this.Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {   
                this.Y -= this.Speed;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                this.Y += this.Speed;
            }

            ScreenLimit(graphics);
        }           
    }
}
