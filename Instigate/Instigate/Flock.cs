#region File Description
//-----------------------------------------------------------------------------
// Flock.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Instigate
{

    public struct AIParameters
    {
        /// <summary>
        /// how far away the animals see each other
        /// </summary>
        public float DetectionDistance;
        /// <summary>
        /// seperate from animals inside this distance
        /// </summary>
        public float SeparationDistance;
        /// <summary>
        /// how much the animal tends to move in it's previous direction
        /// </summary>
        public float MoveInOldDirectionInfluence;
        /// <summary>
        /// how much the animal tends to move with animals in it's detection distance
        /// </summary>
        public float MoveInFlockDirectionInfluence;
        /// <summary>
        /// how much the animal tends to move randomly
        /// </summary>
        public float MoveInRandomDirectionInfluence;
        /// <summary>
        /// how quickly the animal can turn
        /// </summary>
        public float MaxTurnRadians;
        /// <summary>
        /// how much each nearby animal influences it's behavior
        /// </summary>
        public float PerMemberWeight;
        /// <summary>
        /// how much dangerous animals influence it's behavior
        /// </summary>
        public float PerDangerWeight;
    }

    /// <summary>
    /// This class manages all the Ships in the flock and handles 
    /// their update and draw
    /// </summary>
    public class Flock
    {
        #region Constants
        //Number of FLock members
        const int flockSize = 40;
        #endregion

        #region Fields

        //Ships that fly out of the boundry(screen) will wrap around to 
        //the other side
        int boundryWidth;
        int boundryHeight;

        /// <summary>
        /// List of Flock Members
        /// </summary>
        public List<Ship> flock;

        //Faction for this flock
        public Faction affiliation;

        /// <summary>
        /// Parameters flock members use to move and think
        /// </summary>
        public AIParameters FlockParams
        {
            get
            {
                return FlockParams;
            }

            set
            {
                flockParams = value;
            }           
        }
        protected AIParameters flockParams;
        

        #endregion

        #region Initialization

        /// <summary>
        /// Setup the flock boundaries and generate individual members of the flock
        /// </summary>
        /// <param name="tex"> The texture to be used by the Ships</param>
        /// <param name="screenWidth">Width of the screen</param>
        /// <param name="screenHeight">Height of the screen</param>
        /// <param name="flockParameters">Behavior of the flock</param>
        public Flock(int screenWidth, int screenHeight,
            AIParameters flockParameters, Faction faction)
        {
            boundryWidth = screenWidth;
            boundryHeight = screenHeight;

            affiliation = faction;

            flock = new List<Ship>();
            flockParams = flockParameters;
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Update each flock member, Each Ship want to fly with or flee from everything
        /// it sees depending on what type it is
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="cat"></param>
        public void Update(GameTime gameTime, List<Flock> flocks, List<Ship> ships)
        {
            foreach (Ship thisShip in flock)
            {
                thisShip.ResetThink();

                foreach (Ship otherShip in flock)
                {
                    //this check is so we don't try to fly to ourself!
                    if (thisShip != otherShip)
                    {
                        thisShip.ReactTo(otherShip, ref flockParams);
                    }
                }

                //Look for the other flocks
                foreach (Flock flock1 in flocks)
                {
                    if (flock1 != this)
                        thisShip.ReactTo(flock1, ref flockParams);
                }
                
                thisShip.Update(gameTime, ref flockParams, ships);
            }
        }

        /// <summary>
        /// Calls Draw on every member of the Flock
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Ship theShip in flock)
            {
                theShip.Draw(spriteBatch);
            }
        }

        #endregion
    }
}
