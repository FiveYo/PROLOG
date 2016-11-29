using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SbsSW.SwiPlCs;


namespace TarotAfricain.Core
{
    class GameObject
    {
        public Rectangle Position;
        public Texture2D Texture;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
