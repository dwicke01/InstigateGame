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
    // ship class
    public class Ship
    {

        #region FIELDS
        // for flocking
        Vector2 aiNewDir;
        int aiNumSeen;

        public Texture2D sprite;
        public Vector2 velocity { get; set; }
        public bool blastersOn; // checks if the ship is chasing
        
        public Vector2 Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }
        protected Vector2 location;
        
        public Faction affiliation;
        Vector2 shipCenter;
        public float orientation;
        public bool atWar;
        public Ship nearestEnemy;

        Random random = new Random();

        Dictionary<string, Behaviors> behaviors;

        public float ReactionDistance
        {
            get
            {
                return reactionDistance;
            }
        }
        protected float reactionDistance;

        public Vector2 ReactionLocation
        {
            get
            {
                return reactionLocation;
            }
        }
        protected Vector2 reactionLocation;

        public int BoundryWidth
        {
            get
            {
                return boundryWidth;
            }
        }
        protected int boundryWidth;

        public int BoundryHeight
        {
            get
            {
                return boundryHeight;
            }
        }
        protected int boundryHeight;

        public Vector2 Direction
        {
            get
            {
                return direction;
            }
        }
        protected Vector2 direction;

        protected float moveSpeed;

        #endregion

        public Ship(int x, int y, Texture2D Sprite, Faction faction, List<Ship> ships) {
            sprite = Sprite;
            velocity = new Vector2(2, 1);
            Location = new Vector2(x, y);
            affiliation = faction;
            orientation = 1f;
            shipCenter = new Vector2(sprite.Width/2, sprite.Height/2);
            moveSpeed = .5f;
            boundryWidth = 800;
            boundryHeight = 480;
            blastersOn = false;
            behaviors = new Dictionary<string, Behaviors>();
            BuildBehaviors();
            if (affiliation.feuds.Count == 0)
            {
                atWar = false;
                nearestEnemy = null;
            }
            else
            {
                atWar = true;
                findNearestEnemy(ships);
            }  
        }

        private void findNearestEnemy(List<Ship> ships)
        {
            nearestEnemy = null;
            foreach (Ship ship in ships)
            {
                if (nearestEnemy == null && affiliation.feuds.Contains(ship.affiliation))
                    nearestEnemy = ship;
                else if (affiliation.feuds.Contains(ship.affiliation) && 
                    Vector2.Distance(location, ship.location) < Vector2.Distance(location, nearestEnemy.location))
                    nearestEnemy = ship;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
           orientation = (float)Math.Atan2(direction.Y, direction.X);
           spriteBatch.Draw(sprite, Location, null, affiliation.color, orientation, shipCenter, 1.0f, SpriteEffects.None, 0.0f);
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)Location.X, (int)Location.Y, sprite.Width, sprite.Height);
        }

        public void Update(GameTime gameTime, ref AIParameters aiParams, List<Ship> ships)
        {
            findNearestEnemy(ships);
            if (nearestEnemy == null)
            {
                blastersOn = false;
                moveSpeed = 0.5f;
            }

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            Vector2 randomDir = Vector2.Zero;

            randomDir.X = (float)random.NextDouble() - 0.5f;
            randomDir.Y = (float)random.NextDouble() - 0.5f;
            Vector2.Normalize(ref randomDir, out randomDir);

            aiNumSeen = ships.Count - 1;

            if (aiNumSeen > 0)
            {
                aiNewDir = (direction * aiParams.MoveInOldDirectionInfluence) +
                    (aiNewDir * (aiParams.MoveInFlockDirectionInfluence / 
                    (float)aiNumSeen));
            }
            else
            {
                aiNewDir = direction * aiParams.MoveInOldDirectionInfluence;
            }

            aiNewDir += (randomDir * aiParams.MoveInRandomDirectionInfluence);
            Vector2.Normalize(ref aiNewDir, out aiNewDir);
            aiNewDir = ChangeDirection(direction, aiNewDir, 
                aiParams.MaxTurnRadians*elapsedTime);
            direction = aiNewDir;

            if (direction.LengthSquared() > .01f)
            {
                Vector2 moveAmount = direction * moveSpeed * 2;
                location = location + moveAmount;

                if (location.X < 0.0f)
                {
                    location.X = boundryWidth + location.X;
                }
                else if (location.X > boundryWidth)
                {
                    location.X = location.X - boundryWidth;
                }

                location.Y += direction.Y * moveSpeed * 2;
                if (location.Y < 0.0f)
                {
                    location.Y = boundryHeight + Location.Y;
                }
                else if (location.Y > boundryHeight)
                {
                    location.Y -= boundryHeight;
                }
            }
        
        }

        private void ClosestLocation(
            ref Vector2 destLocation, out Vector2 outLocation)
        {
            outLocation = new Vector2();
            float x = destLocation.X;
            float y = destLocation.Y;
            float dX = Math.Abs(destLocation.X - Location.X);
            float dY = Math.Abs(destLocation.Y - Location.Y);

            if (Math.Abs(boundryWidth - destLocation.X + Location.X) < dX)
            {
                dX = boundryWidth - destLocation.X + Location.X;
                x = destLocation.X - boundryWidth;
            }
            if (Math.Abs(boundryWidth - Location.X + destLocation.X) < dX)
            {
                dX = boundryWidth - Location.X + destLocation.X;
                x = destLocation.X + boundryWidth;
            }

            if (Math.Abs(boundryHeight - destLocation.Y + Location.Y) < dY)
            {
                dY = boundryHeight - destLocation.Y + Location.Y;
                y = destLocation.Y - boundryHeight;
            }
            if (Math.Abs(boundryHeight - Location.Y + destLocation.Y) < dY)
            {
                dY = boundryHeight - Location.Y + destLocation.Y;
                y = destLocation.Y + boundryHeight;
            }
            outLocation.X = x;
            outLocation.Y = y;
        }
          
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public void ReactTo(Flock flock, ref AIParameters AIparams)
        {
            if (flock != null)
            {
                foreach (Ship ship in flock.flock)
                {
                    if (ship == nearestEnemy)
                    {
                        ReactTo(ship, ref AIparams);
                        if (Vector2.Distance(location, ship.location) > 40.0f)
                        {
                            moveSpeed = 1.0f;
                            blastersOn = false;
                        }
                        else
                        {
                            moveSpeed = 0.5f;
                            blastersOn = true;
                        }
                    }
                }
            }
        }

        public Matrix GetTransform()
        {
            Matrix transform = Matrix.Identity;
            transform *= Matrix.CreateTranslation(-sprite.Width / 2, -sprite.Height / 2, 0);
            transform *= Matrix.CreateRotationZ(-orientation);
            transform *= Matrix.CreateTranslation(new Vector3(Location, 0));
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

        public Laser Fire()
        {
            return new Laser(location, direction, affiliation);
        }

        public void ReactTo(Ship ship, ref AIParameters AIparams)
        {
            if (ship != null)
            {
                Vector2 otherLocation = ship.Location;
                ClosestLocation(ref otherLocation,
                    out reactionLocation);
                reactionDistance = Vector2.Distance(Location, reactionLocation);

                // only react if the ship is close enough that it's seen, but reaction distance is whole map
                if (reactionDistance < AIparams.DetectionDistance)
                {
                    Behaviors reactions = new Behaviors();
                    if (affiliation.allies.Contains(ship.affiliation))
                        reactions = behaviors["ally"];
                    else if (affiliation.feuds.Contains(ship.affiliation))
                        reactions = behaviors["enemy"];
                    else
                        reactions = behaviors["ally"];
                    foreach (Behavior reaction in reactions)
                    {
                        reaction.Update(ship, AIparams);
                        if (reaction.Reacted)
                        {
                            aiNewDir += reaction.Reaction;
                            aiNumSeen++;
                        }
                    }
                }
            }
        }

        public void ResetThink()
        {
            aiNewDir = Vector2.Zero;
            aiNumSeen = 0;
            reactionDistance = 0f;
            reactionLocation = Vector2.Zero;
        }

        public void BuildBehaviors()
        {
            Behaviors enemyReactions = new Behaviors();
            enemyReactions.Add(new AttackBehavior(this)); 
            behaviors.Add("enemy", enemyReactions);

            Behaviors allyReactions = new Behaviors();
            allyReactions.Add(new AlignBehavior(this));
            allyReactions.Add(new CohesionBehavior(this));
            allyReactions.Add(new SeparationBehavior(this));
            behaviors.Add("ally", allyReactions);
        }

        private static Vector2 ChangeDirection(
            Vector2 oldDir, Vector2 newDir, float maxTurnRadians)
        {
            float oldAngle = (float)Math.Atan2(oldDir.Y, oldDir.X);
            float desiredAngle = (float)Math.Atan2(newDir.Y, newDir.X);
            float newAngle = MathHelper.Clamp(desiredAngle, WrapAngle(
                    oldAngle - maxTurnRadians), WrapAngle(oldAngle + maxTurnRadians));
            return new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));
        }
    }
}
