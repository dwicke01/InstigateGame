using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Instigate
{
    // behavior for when ships are at war
    class AttackBehavior : Behavior
    {
        public AttackBehavior(Ship Ship)
            : base(Ship)
        {
        }

        public override void Update(Ship otherShip, AIParameters aiParams)
        {
            base.ResetReaction();


            Vector2 dangerDirection = Vector2.Zero;

            
            if (Vector2.Dot(
                Ship.Location, Ship.ReactionLocation) >= -(Math.PI / 2))
            {
                reacted = true;

                dangerDirection = Ship.Location - Ship.ReactionLocation;
                Vector2.Normalize(ref dangerDirection, out dangerDirection);

                reaction = (aiParams.PerDangerWeight * 100000 * dangerDirection);

                Vector2.Negate(ref reaction, out reaction);
            }
        }
    }
}
