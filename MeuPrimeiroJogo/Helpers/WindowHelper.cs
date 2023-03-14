using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asteroid.Models.Elements;
using Microsoft.Xna.Framework.Input;
using Asteroid.Models.Characters;

namespace Asteroid.Helpers
{
    public static class WindowHelper
    {
        /// <summary>
        /// Retorna X e Y do centro da tela
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns>X e Y</returns>
        public static (int, int) GetCenters(this GraphicsDeviceManager graphics)
        {
            var x = (graphics.PreferredBackBufferWidth - 200) / 2;
            var y = (graphics.PreferredBackBufferHeight - 50) / 2;
            return (x, y);
        }

        public static int GetCenterX(this GraphicsDeviceManager graphics)
        {
            var x = (graphics.PreferredBackBufferWidth - 200) / 2;            
            return x;
        }

        public static int GetCenterY(this GraphicsDeviceManager graphics)
        {            
            var y = (graphics.PreferredBackBufferHeight - 50) / 2;
            return y;
        }

        public static void DrawElement(this SpriteBatch spriteBatch, Element element)
        {
            spriteBatch.Draw(element.Texture, element.Rectangle, element.Color);            
        }

 
        public static void DrawText(this SpriteBatch spriteBatch, Text text)
        {
            spriteBatch.DrawString(text.SpriteFont, text.Content, text.Vector, text.Color, text.Roatation, text.Origin, text.Scale, text.SpriteEffects, text.LayerDepth);
        }
    }
}
