#region File Description
//-----------------------------------------------------------------------------
// SeparationBehavior.cs
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
    /// SeparationBehavior is a Behavior that will make an Ship move away from
    /// another if it's too close for comfort
    /// </summary>
    class SeparationBehavior : Behavior
    {
        #region Initialization
        public SeparationBehavior(Ship Ship)
            : base(Ship)
        {
        }
        #endregion

        #region Update

        /// <summary>
        /// separationBehavior.Update infuences the owning Ship to move away from
        /// the otherShip is it’s too close, in this case if it’s inside 
        /// AIParameters.separationDistance.
        /// </summary>
        /// <param name="otherShip">the Ship to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public override void Update(Ship otherShip, AIParameters aiParams)
        {
            base.ResetReaction();

            Vector2 pushDirection = Vector2.Zero;
            float weight = aiParams.PerMemberWeight;

            if (Ship.ReactionDistance > 0.0f && 
                Ship.ReactionDistance <= aiParams.SeparationDistance)
            {
                //The otherShip is too close so we figure out a pushDirection 
                //vector in the opposite direction of the otherShip and then weight
                //that reaction based on how close it is vs. our separationDistance

                pushDirection = Ship.Location - Ship.ReactionLocation;
                Vector2.Normalize(ref pushDirection, out pushDirection);

                //push away
                weight *= (1 - 
                    (float)Ship.ReactionDistance / aiParams.SeparationDistance);

                pushDirection *= weight;

                reacted = true;
                reaction += pushDirection;
            }
        }
        #endregion
    }
}
