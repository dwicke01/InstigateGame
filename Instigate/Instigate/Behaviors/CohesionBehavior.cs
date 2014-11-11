#region File Description
//-----------------------------------------------------------------------------
// CohesionBehavior.cs
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
    /// CohesionBehavior is a Behavior that  makes an Ship move towards another 
    /// if it's not already too close
    /// </summary>
    class CohesionBehavior : Behavior
    {
        #region Initialization
        public CohesionBehavior(Ship Ship)
            : base(Ship)
        {
        }
        #endregion

        #region Update

        /// <summary>
        /// CohesionBehavior.Update infuences the owning Ship to move towards the
        /// otherShip that it sees as long as it isn’t too close, in this case 
        /// that means inside the separationDist in the passed in AIParameters.
        /// </summary>
        /// <param name="otherShip">the Ship to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public override void Update(Ship otherShip, AIParameters aiParams)
        {
            base.ResetReaction();

            Vector2 pullDirection = Vector2.Zero;
            float weight = aiParams.PerMemberWeight;

            //if the otherShip is too close we dont' want to fly any
            //closer to it
            if (Ship.ReactionDistance > 0.0f
                && Ship.ReactionDistance > aiParams.SeparationDistance)
            {
                //We want to make the Ship move closer the the otherShip so we 
                //create a pullDirection vector pointing to the otherShip bird and 
                //weigh it based on how close the otherShip is relative to the 
                //AIParameters.separationDistance.
                pullDirection = -(Ship.Location - Ship.ReactionLocation);
                Vector2.Normalize(ref pullDirection, out pullDirection);

                weight *= (float)Math.Pow((double)
                    (Ship.ReactionDistance - aiParams.SeparationDistance) /
                        (aiParams.DetectionDistance - aiParams.SeparationDistance), 2);

                pullDirection *= weight;

                reacted = true;
                reaction = pullDirection;
            }
        }
        #endregion
    }
}
