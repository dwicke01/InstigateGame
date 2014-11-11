#region File Description
//-----------------------------------------------------------------------------
// Behavior.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace Instigate
{
    /// <summary>
    /// Behavior is the base class for the four flock behaviors in this sample: 
    /// aligning, cohesion, separation and fleeing. It is an abstract class, 
    /// leaving the implementation of Update up to its subclasses. Ship objects 
    /// can have an arbitrary number of behaviors, after the entity calls Update 
    /// on the behavior the reaction results are stored in reaction so the owner 
    /// can query it.
    /// </summary>
    public abstract class Behavior
    {
        #region Fields
        /// <summary>
        /// Keep track of the Ship that this behavior belongs to.
        /// </summary>
        public Ship Ship
        {
            get { return ship; }
            set { ship = value; }
        }
        private Ship ship;

        /// <summary>
        /// Store the behavior reaction here.
        /// </summary>
        public Vector2 Reaction
        {
            get { return reaction; }
        }
        protected Vector2 reaction;

        /// <summary>
        /// Store if the behavior has reaction results here.
        /// </summary>
        public bool Reacted
        {
            get { return reacted; }
        }
        protected bool reacted;
        #endregion

        #region Initialization
        protected Behavior(Ship ship)
        {
            this.ship = ship;
        }
        #endregion

        #region Update
        /// <summary>
        /// Abstract function that the subclass must impliment. Figure out the 
        /// Behavior reaction here.
        /// </summary>
        /// <param name="otherShip">the Ship to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public abstract void Update(Ship otherShip, AIParameters aiParams);

        /// <summary>
        /// Reset the behavior reactions from the last Update
        /// </summary>
        protected void ResetReaction()
        {
            reacted = false;
            reaction = Vector2.Zero;
        }
        #endregion
    }
}
