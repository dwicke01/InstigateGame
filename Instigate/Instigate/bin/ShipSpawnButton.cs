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
    class ShipSpawnButton
    {
        public Texture2D sprite;
        public Texture2D spriteGlow;
        public bool selected;
        public Color color { get; set; }
        public Vector2 location { get; set; }

        public ShipSpawnButton(Texture2D spriteGlow, Texture2D shipSprite, Vector2 location)
        {
            this.spriteGlow = spriteGlow;
            sprite = shipSprite;
            this.location = location;
            color = Color.White;
            selected = false;
        }

        public ShipSpawner onClick()
        {
            if (!selected)
            {
                ShipSpawner spawner = new ShipSpawner(this);
                selected = true;
                return spawner;
            }
            selected = false;
            return null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, getRectangle(), color);
            if (selected)
                spriteBatch.Draw(spriteGlow, getRectangle(), Color.White);
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)location.X, (int)location.Y, sprite.Width, sprite.Height);
        }
    }
}