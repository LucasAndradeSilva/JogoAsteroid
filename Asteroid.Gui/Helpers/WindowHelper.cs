using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asteroid.Gui.Models.Elements;
using Microsoft.Xna.Framework.Input;

namespace Asteroid.Gui.Helpers
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

        public static void DrawBackground(this SpriteBatch spriteBatch, Background background)
        {                        
            spriteBatch.Draw(background.Texture, background.Rectangle, background.Color);            
        }

        public static Texture2D RotateTexture(this Texture2D texture, float rotation)
        {
            float rotationRadians = MathHelper.ToRadians(rotation);
            float cos = (float)Math.Cos(rotationRadians);
            float sin = (float)Math.Sin(rotationRadians);

            int originalWidth = texture.Width;
            int originalHeight = texture.Height;

            // calculate the rotated texture size
            int rotatedWidth = (int)Math.Round(Math.Abs(originalWidth * cos) + Math.Abs(originalHeight * sin));
            int rotatedHeight = (int)Math.Round(Math.Abs(originalHeight * cos) + Math.Abs(originalWidth * sin));

            // create a new texture to hold the rotated image
            Texture2D rotatedTexture = new Texture2D(texture.GraphicsDevice, rotatedWidth, rotatedHeight);

            // create a color array to hold the pixel data for the original texture
            Color[] originalColors = new Color[originalWidth * originalHeight];
            texture.GetData(originalColors);

            // create a color array to hold the pixel data for the rotated texture
            Color[] rotatedColors = new Color[rotatedWidth * rotatedHeight];

            // center of the original texture
            Vector2 center = new Vector2(originalWidth / 2f, originalHeight / 2f);

            // calculate the top-left corner of the rotated texture
            Vector2 topLeft = new Vector2(rotatedWidth / 2f, rotatedHeight / 2f) - center;

            // loop through each pixel of the rotated texture
            for (int y = 0; y < rotatedHeight; y++)
            {
                for (int x = 0; x < rotatedWidth; x++)
                {
                    // calculate the position of the pixel in the original texture
                    Vector2 position = new Vector2(x, y) - topLeft;

                    // rotate the position of the pixel around the center of the texture
                    Vector2 rotatedPosition = new Vector2(
                        position.X * cos - position.Y * sin,
                        position.X * sin + position.Y * cos);

                    // add the center offset to get the final position in the rotated texture
                    rotatedPosition += center;

                    // get the color of the pixel from the original texture
                    Color color = Color.Transparent;
                    if (rotatedPosition.X >= 0 && rotatedPosition.X < originalWidth &&
                        rotatedPosition.Y >= 0 && rotatedPosition.Y < originalHeight)
                    {
                        int index = (int)(rotatedPosition.X + rotatedPosition.Y * originalWidth);
                        color = originalColors[index];
                    }

                    // set the color of the pixel in the rotated texture
                    int rotatedIndex = x + y * rotatedWidth;
                    rotatedColors[rotatedIndex] = color;
                }
            }

            // set the pixel data for the rotated texture and return it
            rotatedTexture.SetData(rotatedColors);
            return rotatedTexture;
        }


    }
}
