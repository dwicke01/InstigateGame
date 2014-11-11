using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Instigate
{
    class FactionButton
    {
        private Texture2D factionSpriteNoGlow;
        private Texture2D factionSpriteGlow;
        public Vector2 position;
        private Color color;
        public bool glow;
        int x;
        int y;

        public FactionButton(ContentManager Content, int x, int y, Color theColor, Camera2d camera)
        {
            factionSpriteGlow = Content.Load<Texture2D>(@"SpawnMenu/FactionButtonGlow2");
            factionSpriteNoGlow = Content.Load<Texture2D>(@"SpawnMenu/FactionButtonNoGlow2");
            this.x = x;
            this.y = y;
            position = Vector2.Transform(new Vector2(x, y), Matrix.Invert(camera.GetTransformation()));
            
            glow = false;
            color = theColor;
        }

        public Color getColor()
        {
            return color;
        }

        public void onClick()
        {
            glow = !glow;
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)position.X, (int)position.Y, factionSpriteGlow.Width, factionSpriteGlow.Height);
        }

        public void Update(Camera2d camera)
        {
            position = Vector2.Transform(new Vector2(x, y), Matrix.Invert(camera.GetTransformation()));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (glow)
                spriteBatch.Draw(factionSpriteGlow, new Rectangle((int)position.X, (int)position.Y, factionSpriteGlow.Width, factionSpriteGlow.Height), color);
            else
                spriteBatch.Draw(factionSpriteNoGlow, new Rectangle((int)position.X, (int)position.Y, factionSpriteGlow.Width, factionSpriteGlow.Height), color);
        }
    }
}
