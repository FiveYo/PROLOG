using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarotAfricain.Core
{
    class ObjectWithText : GameObject
    {
        public SpriteFont font;
        public Vector2 PositionText;
        public string text;

        public void DrawString(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, PositionText, Color.Black);
        }

        public void DrawObject(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch);
            DrawString(spriteBatch);
        }

        public void Dispose()
        {
            this.Texture.Dispose();
            this.text = "";
        }
    }
}
