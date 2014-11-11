using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Instigate
{
    // laser class
    public class Laser
    {
        public Texture2D sprite { get; set; }
        public Vector2 position { get; set; }
        public Vector2 Velocity { get; set; }
        public Faction faction;
        const float LASER_SPEED = 40.0f;
        const float LASER_LIFESPAN = 0.85f;

        float timeUntilDisappears;
        public bool IsDestroyed
        {
            get;
            set;
        }

        public Laser(Vector2 newLocation, Vector2 dir, Faction faction)
        {
            position = newLocation;
            timeUntilDisappears = LASER_LIFESPAN;
            Velocity = dir;
            this.faction = faction;
        }

        public void Load(Texture2D laserTexture)
        {
            sprite = laserTexture;
        }

        public void Update(GameTime gameTime)
        {
            timeUntilDisappears -= gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (timeUntilDisappears <= 0.0f)
                IsDestroyed = true;
            position += Velocity*LASER_SPEED;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }

        public Matrix GetTransform()
        {
            Matrix transform = Matrix.Identity;
            transform *= Matrix.CreateTranslation(-sprite.Width / 2, -sprite.Height / 2, 0);
            transform *= Matrix.CreateTranslation(new Vector3(position, 0));
            return transform;
        }

        //Creates a rectangle completely containing the transformed object
        Rectangle GetTransformedBounds(Vector2 topLeft, Vector2 bottomRight, Matrix transform)
        {
            //Find the two corners not passed to the function
            Vector2 topRight = new Vector2(bottomRight.X, topLeft.Y);
            Vector2 bottomLeft = new Vector2(topLeft.X, bottomRight.Y);

            //Find the locations of the corners post-transformation
            topLeft = Vector2.Transform(topLeft, transform);
            topRight = Vector2.Transform(topRight, transform);
            bottomLeft = Vector2.Transform(bottomLeft, transform);
            bottomRight = Vector2.Transform(bottomRight, transform);

            //Find two corners of the bounding rectangle
            Vector2 boundingTopLeft = Vector2.Min(Vector2.Min(topLeft, topRight), Vector2.Min(bottomLeft, bottomRight));
            Vector2 boundingBottomRight = Vector2.Max(Vector2.Max(topLeft, topRight), Vector2.Max(bottomLeft, bottomRight));

            return new Rectangle(
                (int)boundingTopLeft.X,
                (int)boundingTopLeft.Y,
                (int)boundingBottomRight.X - (int)boundingTopLeft.X,
                (int)boundingBottomRight.Y - (int)boundingTopLeft.Y);
        }

        public bool IsCollidingWith(Ship other)
        {
            Matrix transformToOtherCoordinates = GetTransform() * Matrix.Invert(other.GetTransform());

            Rectangle myBounds = GetTransformedBounds(Vector2.Zero, new Vector2(sprite.Width, sprite.Height), GetTransform());
            Rectangle yourBounds = GetTransformedBounds(Vector2.Zero, new Vector2(other.sprite.Width, other.sprite.Height), other.GetTransform());
            if (!myBounds.Intersects(yourBounds))
                return false;

            Color[] first = new Color[sprite.Width * sprite.Height];
            sprite.GetData(first);
            Color[] second = new Color[other.sprite.Width * other.sprite.Height];
            other.sprite.GetData(second);

            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformToOtherCoordinates);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformToOtherCoordinates);

            Vector2 yPositionInOther = Vector2.Transform(Vector2.Zero, transformToOtherCoordinates);

            for (int yOfFirst = 0; yOfFirst < sprite.Height; ++yOfFirst)
            {
                Vector2 positionInOther = yPositionInOther;
                for (int xOfFirst = 0; xOfFirst < sprite.Width; ++xOfFirst)
                {
                    int xOfSecond = (int)Math.Round(positionInOther.X);
                    int yOfSecond = (int)Math.Round(positionInOther.Y);

                    if (xOfSecond >= 0 && xOfSecond < other.sprite.Width
                        && yOfSecond >= 0 && yOfSecond < other.sprite.Height)
                    {
                        Color colorInFirst = first[xOfFirst + yOfFirst * sprite.Width];
                        Color colorInSecond = second[xOfSecond + yOfSecond * sprite.Width];
                        if (colorInFirst.A != 0 && colorInSecond.A != 0)
                            return true;
                    }
                    positionInOther += stepX;
                }
                yPositionInOther += stepY;
            }
            return false;
        }
    }
}
