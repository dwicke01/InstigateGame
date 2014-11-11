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
    class Map : Microsoft.Xna.Framework.Game
    {
        public List<Ship> ships;
        public List<Bomb> bombs;
        public List<Faction> factions;
        public List<Flock> flocks;
        public List<Laser> lasers;
        public Texture2D background;
        KeyboardState previousState;
        bool paused;
        AIParameters aiparams;
        public Texture2D laserTexture;

        // initializes the map
        public Map(Texture2D background, Texture2D laserTexture)
        {
            factions = new List<Faction>();
            lasers = new List<Laser>();
            factions.Add(new Faction(Color.Blue));
            factions.Add(new Faction(Color.Green));
            factions.Add(new Faction(Color.Red));
            factions.Add(new Faction(Color.Orange));
            this.background = background;
            ships = new List<Ship>();
            bombs = new List<Bomb>();
            flocks = new List<Flock>();
            paused = false;
            previousState = Keyboard.GetState();
            setAIParameters();
            initializeFlocks();
            Content.RootDirectory = "Content";
            this.laserTexture = laserTexture;
        }

        void initializeFlocks()
        {
            flocks.Add(new Flock(800, 480, aiparams, factions[0]));
            flocks.Add(new Flock(800, 480, aiparams, factions[1]));
            flocks.Add(new Flock(800, 480, aiparams, factions[2]));
            flocks.Add(new Flock(800, 480, aiparams, factions[3]));
        }

        public void setAIParameters()
        {
            aiparams.DetectionDistance = 1000000000.0f;
            aiparams.SeparationDistance = 50.0f;
            aiparams.MoveInOldDirectionInfluence = 1.0f;
            aiparams.MoveInFlockDirectionInfluence = 1.0f;
            aiparams.MoveInRandomDirectionInfluence = 0.05f;
            aiparams.PerMemberWeight = 1.0f;
            aiparams.PerDangerWeight = 50000000.0f;
            aiparams.MaxTurnRadians = 6.0f;
        }

        // a ship is created from a spawner
        public void createShip(int x, int y, ShipSpawner spawner)
        {
            Color color = new Color();

            if (spawner.color == Color.LightBlue)
                color = Color.Blue;
            else if (spawner.color == Color.PaleVioletRed)
                color = Color.Red;
            else if (spawner.color == Color.LightGreen)
                color = Color.Green;
            else
                color = Color.Orange;

            Ship ship = new Ship((int)spawner.location.X, (int)spawner.location.Y, spawner.sprite, getFactionFromColor(color), ships);

            ships.Add(ship);
            foreach (Flock flock in flocks)
                if (flock.affiliation == ship.affiliation)
                    flock.flock.Add(ship);
        }

        public Faction getFactionFromColor(Color color)
        {
            foreach (Faction faction in factions)
                if (faction.color == color)
                    return faction;
            return null;
        }

        public void createBomb(int x, int y, BombSpawner spawner)
        {
            Color color = new Color();

            if (spawner.color == Color.LightBlue)
                color = Color.Blue;
            else if (spawner.color == Color.PaleVioletRed)
                color = Color.Red;
            else if (spawner.color == Color.LightGreen)
                color = Color.Green;
            else
                color = Color.Orange;

            Bomb bomb = new Bomb((int)spawner.location.X, (int)spawner.location.Y, spawner.sprite, getFactionFromColor(color));

            bombs.Add(bomb);
        }

        public void Update(GameTime gameTime)
        {
            if (!paused)
            {
                foreach (Flock flock in flocks)
                    flock.Update(gameTime, flocks, ships);

                int k = 0;
                while (k < ships.Count)
                {
                    if (ships[k].blastersOn)
                    {
                        lasers.Add(ships[k].Fire());
                        lasers[lasers.Count - 1].Load(laserTexture);
                        
                        // handles flock attacking
                        foreach (Flock flock in flocks)
                            if (ships[k].nearestEnemy != null && flock.affiliation == ships[k].nearestEnemy.affiliation)
                                flock.flock.Remove(ships[k].nearestEnemy);
                        ships[k].nearestEnemy.affiliation.addFeud(ships[k].affiliation);
                        foreach (Ship ship in ships) // forges alliances
                            if (ship.affiliation.feuds.Contains(ships[k].affiliation))
                            {
                                ship.affiliation.addAlly(ships[k].nearestEnemy.affiliation);
                                ships[k].nearestEnemy.affiliation.addAlly(ship.affiliation);
                            }
                        ships[k].blastersOn = false;
                        Ship temp = ships[k].nearestEnemy;
                        ships[k].nearestEnemy = null;
                        ships.Remove(temp);
                        k = 0;
                    }
                    k++;
                }
                // handles lasers
                for (int i = 0; i < lasers.Count; ++i)
                {
                    if (!lasers[i].IsDestroyed)
                    {
                        lasers[i].Update(gameTime);
                        for (int j = 0; j < ships.Count; ++j)
                        {
                            if (lasers[i].faction != ships[j].affiliation && lasers[i].IsCollidingWith(ships[j]))
                            {
                                lasers.RemoveAt(i--);
                                foreach (Flock flock in flocks)
                                    if (flock.flock.Contains(ships[j]))
                                        flock.flock.Remove(ships[j]);
                                ships.RemoveAt(j--);
                                break;
                            }
                        }
                    }
                    else
                        lasers.RemoveAt(i--);
                }
                // handles bombs
                for (int i = 0; i < bombs.Count; i++)
                {
                    if (bombs[i].hasExploded(gameTime))
                    {
                        for (int j = 0; j < ships.Count; j++)
                        {
                            if (ships[j].getRectangle().Intersects(bombs[i].getRectangle()))
                            {
                                foreach (Faction faction in factions)
                                    if (faction.color == ships[j].affiliation.color)
                                        faction.addFeud(bombs[i].affiliation);
                                ships[j].affiliation.addFeud(bombs[i].affiliation);
                                foreach (Ship ship in ships)
                                    if (ship.affiliation.feuds.Contains(ships[j].affiliation))
                                    {
                                        ship.affiliation.addAlly(ships[k].nearestEnemy.affiliation);
                                        ships[k].nearestEnemy.affiliation.addAlly(ship.affiliation);
                                    }
                                foreach (Flock flock in flocks)
                                    if (flock.flock.Contains(ships[j]))
                                        flock.flock.Remove(ships[j]);
                                ships.RemoveAt(j);
                                j--;
                            }
                        }
                        bombs.RemoveAt(i);
                        i--;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space))
                    paused = true;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space))
                    paused = false;
            }
            previousState = Keyboard.GetState();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
            foreach (Ship ship in ships)
                ship.Draw(spriteBatch);
            foreach (Bomb bomb in bombs)
                bomb.Draw(spriteBatch);
            foreach (Laser laser in lasers)
                laser.Draw(spriteBatch);
        }
    }
}
