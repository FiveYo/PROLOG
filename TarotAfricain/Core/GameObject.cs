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
    public class GameObject
    {
        public Rectangle Position;
        public Texture2D Texture;

        public void Draw(SpriteBatch spriteBatch)
        {
            try
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
            catch (System.NullReferenceException) { }
            catch (System.ArgumentNullException) { }
        }

        public void Dispose()
        {
            this.Texture.Dispose();
        }

        public void Hide()
        {
            this.Position.Width = 0;
            this.Position.Height = 0;
        }

        public void Show()
        {
            this.Position.Width = this.Texture.Width;
            this.Position.Height = this.Texture.Height;
        }
    }
}
