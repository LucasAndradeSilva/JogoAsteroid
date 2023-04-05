using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Models.Elements
{
    public class TextInputComponent : Element
    {   
        public string TextInput = "";

        private string TextTyped = "";
        private bool isActive = false;
        
        private int TimeBetweenClick = 100;
        private int TimeElapsedCLick = 0;

        public void Update(GameTime gameTime)
        {
            // Verifica se o componente está ativo e se o usuário está digitando
            if (isActive && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                isActive = false;
            }
            else if (isActive)
            {
                if (TimeElapsedCLick > TimeBetweenClick)
                {                    
                    // Adiciona cada caractere digitado pelo usuário ao texto
                    foreach (Keys key in Keyboard.GetState().GetPressedKeys())
                    {
                        if (key == Keys.Back && TextTyped.Length > 0)
                        {
                            // Remove o último caractere digitado se a tecla Backspace for pressionada
                            TextTyped = TextTyped.Remove(TextTyped.Length - 1);
                        }
                        else if (char.IsLetterOrDigit((char)key))
                        {
                            // Adiciona o caractere digitado ao texto se for uma letra ou um número
                            if (TextTyped.Length <= 20)
                            {
                                TextTyped += (char)key;
                            }
                            
                        }
                        else if (key == Keys.Space)
                        {
                            if (TextTyped.Length <= 20)
                            {
                                TextTyped += " ";
                            }
                        }

                        TimeElapsedCLick = 0;
                    }
                }
                else
                {
                    TimeElapsedCLick += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            // Desenha o componente de entrada de texto na tela
            spriteBatch.DrawString(this.SpriteFont, $"{TextInput}{TextTyped}", position, Color.White);
        }

        public void Activate()
        {
            // Ativa o componente de entrada de texto
            isActive = true;
        }

        public string Text
        {
            get { return TextTyped; }
            set { TextTyped = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
        }
    }

}
