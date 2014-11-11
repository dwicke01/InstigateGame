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
    class ShipSpawner
    {
        public Texture2D sprite;
        public Vector2 location;
        public Color color;

        //is initialized from a shipspawnbutton
        public ShipSpawner(ShipSpawnButton button)
        {
            sprite = button.sprite;
            if (button.color == Color.Blue)
                color = Color.LightBlue;
            else if (button.color == Color.Red)
                color = Color.PaleVioletRed;
            else if (button.color == Color.Green)
                color = Color.LightGreen;
            else
                color = Color.OrangeRed;
            location = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }

        public void Update(Camera2d camera)
        {
            Vector2 mouseTransformed = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Matrix.Invert(camera.GetTransformation()));
            location = new Vector2(mouseTransformed.X, mouseTransformed.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, getRectangle(), color);
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)(location.X-sprite.Width/2), (int)(location.Y-sprite.Height/2), sprite.Width, sprite.Height);
        }
    }
}
