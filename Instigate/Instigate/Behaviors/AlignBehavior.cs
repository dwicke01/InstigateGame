#region File Description
//-----------------------------------------------------------------------------
// AlignBehavior.cs
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
    /// AlignBehavior is a Behavior that makes an Ship move in the same
    /// direction that the other Ship it sees is
    /// </summary>
    class AlignBehavior : Behavior
    {
        #region Initialization
        public AlignBehavior(Ship Ship)
            : base(Ship)
        {
        }
        #endregion

        #region Update

        /// <summary>
        /// AlignBehavior.Update infuences the owning Ship to move in same the 
        /// direction as the otherShip that it sees.
        /// </summary>
        /// <param name="otherShip">the Ship to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public override void Update(Ship otherShip, AIParameters aiParams)
        {
            base.ResetReaction();

            if (otherShip != null)
            {
                reacted = true;
                reaction = (otherShip.velocity * aiParams.PerMemberWeight);
            }
        }
        #endregion
    }
}
