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
    // bomb class
    class Bomb
    {
        public Texture2D sprite;
        public Vector2 velocity { get; set; }
        public Vector2 position { get; set; }
        public Faction affiliation;
        private const double DURATION = 900.0f;
        private double timer;

        public Bomb(int x, int y, Texture2D Sprite, Faction faction)
        {
            sprite = Sprite;
            velocity = new Vector2(2, 1);
            position = new Vector2(x, y);
            affiliation = faction;
            timer = DURATION;
        }

        // checks the timer to see if the bomb has exploded
        public bool hasExploded(GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.Milliseconds;
            if (timer <= 0)
                return true;
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, getRectangle(), affiliation.color);
            //spriteBatch.Draw(sprite, position, null, affiliation.color, 4.0f, new Vector2(position.X+50, position.Y+50), 1.0f, SpriteEffects.None, 0.0f);

        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
        }
    }
}
