using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarotAfricain.Core
{
    class Textbox
    {
        public SpriteFont font;
        public Vector2 Position;
        public string text;

        public Textbox()
        {
            text = "";
        }
        public void DrawString(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, Position, Color.Black);
        }
    }
}
